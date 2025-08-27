using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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

            foreach (var (localTransform, shootAttack, target,unitMover) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShootAttackData>, RefRO<TargetData>,RefRW<UnitMoverData>>())
            {
                if (target.ValueRO.TargetEntity == Entity.Null||!SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
                    continue;
                
                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                
                var distanceSq = math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position);

                if (distanceSq > math.square(shootAttack.ValueRO.AttackDistance))
                {
                    unitMover.ValueRW.TargetPosition = targetLocalTransform.Position;
                    continue; 
                } 
                unitMover.ValueRW.TargetPosition = localTransform.ValueRO.Position; 
                
                float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);
                
                var targetRotation = quaternion.LookRotation(aimDirection, math.up());
                float step = SystemAPI.Time.DeltaTime * unitMover.ValueRO.RotationSpeed;
                var rotSlerp = math.slerp(localTransform.ValueRO.Rotation, targetRotation, step);
                
                localTransform.ValueRW.Rotation = rotSlerp;

                shootAttack.ValueRW.Timer -= dt;
                if (shootAttack.ValueRW.Timer > 0f) 
                    continue; 
                
                shootAttack.ValueRW.Timer = shootAttack.ValueRW.TimerMax;

                
                    
                

                // Spawn via ECB (deferred)
                var bullet = ecb.Instantiate(refs.BulletDataPrefabEntity);

                // Initialize entirely via ECB
                float3 bulletSpawnPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.BulletSpawnLocalPosition);
                ecb.SetComponent(bullet, LocalTransform.FromPosition(bulletSpawnPosition));

                // BulletData
                if (state.EntityManager.HasComponent<BulletData>(refs.BulletDataPrefabEntity))
                {
                    var bulletData = state.EntityManager.GetComponentData<BulletData>(refs.BulletDataPrefabEntity);
                    bulletData.DamageAmount = shootAttack.ValueRO.Damage;
                    ecb.SetComponent(bullet, bulletData);
                }
                else
                    ecb.AddComponent(bullet, new BulletData { DamageAmount = shootAttack.ValueRO.Damage, Speed = 10f });

                // TargetData
                var tgt = new TargetData { TargetEntity = target.ValueRO.TargetEntity };
                if (state.EntityManager.HasComponent<TargetData>(refs.BulletDataPrefabEntity))
                    ecb.SetComponent(bullet, tgt);
                else
                    ecb.AddComponent(bullet, tgt);
            }
        }
    }
}