using Authoring;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public partial struct ShootAttackSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((var shootAttack,var target) 
                     in SystemAPI.Query<RefRW<ShootAttackData>,RefRO<TargetData>>()) {
                
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;
                shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRW.Timer > 0f)
                    continue;
                shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;

                 MakeAttack(ref state,shootAttack,target); 
            }

        } 
        
        private void MakeAttack(ref SystemState state, RefRW<ShootAttackData> shootAttack, RefRO<TargetData> target)
        {
            if (SystemAPI.HasComponent<HealthData>(target.ValueRO.TargetEntity))
            {
                var targetHealth = SystemAPI.GetComponentRW<HealthData>(target.ValueRO.TargetEntity);
                targetHealth.ValueRW.Health -= shootAttack.ValueRO.Damage;


            }
            // Debug.Log($"Shoot! {target.ValueRO.TargetEntity}");
            //throw new System.NotImplementedException();
        }
    }
}