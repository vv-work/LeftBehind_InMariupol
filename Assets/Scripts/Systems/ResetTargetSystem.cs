using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst=true)]
    public partial struct ResetTargetSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var target in SystemAPI.Query<RefRW<TargetData>>()) {
                if (target.ValueRO.TargetEntity != Entity.Null)
                {
                    if (!SystemAPI.HasComponent<TargetData>(target.ValueRO.TargetEntity) ||
                        !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
                    {
                        target.ValueRW.TargetEntity = Entity.Null;
                    }
                } 
            }

        } 
    }
}