using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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
            
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged); 
            var refs = SystemAPI.GetSingleton<EntitiesReferencesData>(); 
            
            foreach ((var spawner,var localTransform) in SystemAPI.Query<RefRW<ZombieSpawnerData>,RefRO<LocalTransform>>()) {
                
                 if (spawner.ValueRO.Timer > 0f) {
                    spawner.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                    continue; 
                 } 
                 spawner.ValueRW.Timer = spawner.ValueRO.TimerMax;
                 var random = spawner.ValueRO.Random;
                 var seed = random.NextUInt();
                 
                 var zombieEntity = ecb.Instantiate(refs.ZombieEntity);
                 
                 ecb.AddComponent(zombieEntity,new RandomWalkingData()
                 {
                     OriginPosition = localTransform.ValueRO.Position,
                     TargetPosition = localTransform.ValueRO.Position, 
                     DistanceMin =  spawner.ValueRO.RandomWalkingDistanceMin,
                     DistanceMax =  spawner.ValueRO.RandomWalkingDistanceMax, 
                     Random = new Unity.Mathematics.Random(seed), 
                 });
                 ecb.SetComponent(zombieEntity,LocalTransform.FromPosition(localTransform.ValueRO.Position)); 
                 spawner.ValueRW.Random = random;
                
            }

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}