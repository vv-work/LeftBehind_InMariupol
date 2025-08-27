using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

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
            
            //todo : write notes on NativeList
            NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
            

            foreach ((var localTransform, var findTarget) in 
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<FindTargetData>>()) { 
                
                CollisionFilter collisionFilter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = GameAssets.UNITY_LAYER,
                    GroupIndex = 0
                
                }
                // todo: write notes on OverlapSphere
                collisionWorld.OverlapSphere(
                    localTransform.ValueRO.Position, 
                    findTarget.ValueRO.Range, 
                    ref distanceHitList,);
                


            }

        }
    }
}
