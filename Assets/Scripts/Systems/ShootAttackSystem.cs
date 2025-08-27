using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    public partial struct ShootAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<EntitiesReferencesData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                               .CreateCommandBuffer(state.WorldUnmanaged);

            var refs = SystemAPI.GetSingleton<EntitiesReferencesData>();
            float dt = SystemAPI.Time.DeltaTime;

            foreach (var (lt, shoot, target) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShootAttackData>, RefRO<TargetData>>())
            {
                if (target.ValueRO.TargetEntity == Entity.Null) continue;

                Debug.Log($"Update");
                shoot.ValueRW.Timer -= dt;
                if (shoot.ValueRW.Timer > 0f) continue;
                
                shoot.ValueRW.Timer = shoot.ValueRW.TimerMax;

                // Spawn via ECB (deferred)
                var bullet = ecb.Instantiate(refs.BulletDataPrefabEntity);

                // Initialize entirely via ECB
                ecb.SetComponent(bullet, LocalTransform.FromPosition(lt.ValueRO.Position));

                // BulletData
                if (state.EntityManager.HasComponent<BulletData>(refs.BulletDataPrefabEntity))
                {
                    ecb.SetComponent(bullet, new BulletData { DamageAmount = shoot.ValueRO.Damage });
                }
                else
                {
                    ecb.AddComponent(bullet, new BulletData { DamageAmount = shoot.ValueRO.Damage });
                }

                // TargetData
                var tgt = new TargetData { TargetEntity = target.ValueRO.TargetEntity };
                if (state.EntityManager.HasComponent<TargetData>(refs.BulletDataPrefabEntity))
                {
                    ecb.SetComponent(bullet, tgt);
                }
                else
                {
                    ecb.AddComponent(bullet, tgt);
                }
            }
        }
    }
}