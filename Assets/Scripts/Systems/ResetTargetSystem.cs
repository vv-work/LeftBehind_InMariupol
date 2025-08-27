using Authoring;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ResetTargetSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var target in SystemAPI.Query<RefRW<TargetData>>()) {

                if (!SystemAPI.HasComponent<TargetData>(target.ValueRO.TargetEntity)) {
                    target.ValueRW.TargetEntity = Entity.Null;
                }
                
            }

        } 
    }
}