using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct FindTargetSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //todo: write notes on PhysicsWorldSingleton and CollisionWorld 
            
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorld.CollisionWorld;
            
            NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
            

            foreach ((var localTransform, var findTarget,var target) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTargetData>,RefRW<TargetData>>())
            {
                findTarget.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.Timer > 0f)
                    continue;
                
                findTarget.ValueRW.Timer = findTarget.ValueRO.TimerMax;
                

                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = GameAssets.UNITY_LAYER,
                    GroupIndex = 0

                };
                distanceHitList.Clear();
                
                // OverlapSphere returns the closest hit
                bool collision =  collisionWorld.OverlapSphere(
                    localTransform.ValueRO.Position, 
                    findTarget.ValueRO.Range, 
                    ref distanceHitList,
                    collisionFilter);
                if (collision)
                { 
                    for (int i = 0; i < distanceHitList.Length; i++)
                    {
                        var hit = distanceHitList[i];
                        if (SystemAPI.HasComponent<UnitData>(hit.Entity))
                        {
                            var otherUnit = SystemAPI.GetComponentRO<UnitData>(hit.Entity);
                            if (findTarget.ValueRO.TargetFaction == otherUnit.ValueRO.Faction) {
                                target.ValueRW.TargetEntity = hit.Entity;
                                break;
                            }
                            
                        }
                    }
                }
            }

        }
    }
}
