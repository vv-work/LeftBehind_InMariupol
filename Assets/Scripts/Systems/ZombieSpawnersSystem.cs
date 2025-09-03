using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    public partial struct ZombieSpawnersSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<EntitiesReferencesData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            
            // var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                // .CreateCommandBuffer(state.WorldUnmanaged);
                var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var refs = SystemAPI.GetSingleton<EntitiesReferencesData>();
            float dt = SystemAPI.Time.DeltaTime;
            
            foreach ((var spawner,var localTransform) in SystemAPI.Query<RefRW<ZombieSpawnerData>,RefRO<LocalTransform>>()) {
                
                if (spawner.ValueRO.Timer > 0f) {
                    spawner.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                    continue; 
                } 
                spawner.ValueRW.Timer = spawner.ValueRO.TimerMax;
                var zombie = ecb.Instantiate(refs.ZombieEntity);
                ecb.SetComponent(zombie,LocalTransform.FromPosition(localTransform.ValueRO.Position)); 
            }

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}