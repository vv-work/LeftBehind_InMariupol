using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct BulletMoverSystem : ISystem 
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            // var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach ((var localTransform, var bullet,var target,var entity) 
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BulletData>,RefRO<TargetData>>().WithEntityAccess()) {

                
                
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;
                
                var  targetPosition = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity).Position;
                var ourPosition = localTransform.ValueRW.Position;
                
                var direction =  math.normalize(targetPosition- localTransform.ValueRO.Position); 
                var moveOffset = direction * bullet.ValueRO.Speed * deltaTime;
                var distanceBefore = math.distancesq(ourPosition, targetPosition);
                
                localTransform.ValueRW.Position += moveOffset; 
                var distanceAfter = math.distancesq(localTransform.ValueRO.Position, targetPosition);
                
                if (distanceBefore < 2*math.lengthsq(moveOffset) || distanceBefore < distanceAfter ) { 
                    
                    if (SystemAPI.HasComponent<HealthData>(target.ValueRO.TargetEntity))
                    { 
                        var health = SystemAPI.GetComponentRW<HealthData>(target.ValueRO.TargetEntity);
                        health.ValueRW.Health += bullet.ValueRO.DamageAmount; 
                    }
                    ecb.DestroyEntity(entity);
                    
                }

            }
        }

    }
}