using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct HealthDeathTestSystem : ISystem {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
                // new EntityCommandBuffer(Allocator.Temp); 
            
            foreach ((var health, var entity) in SystemAPI.Query<RefRW<HealthData>>().WithEntityAccess()) { 
                
                if (health.ValueRO.Health <= 0) {
                    
                    ecb.DestroyEntity(entity); 
                }
                
            } 
        }

    }
}