using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    // [UpdateBefore(typeof(ResetEventsSystem))]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventsSystem))]
    public partial struct SelectVisualSystem : ISystem
    { 
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((var selected,var entity) in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>().WithEntityAccess())
            { 
                 if (selected.ValueRO.OnSelected) { 
                     var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                     visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
                 }

                 if (selected.ValueRO.OnDeselected) { 
                     var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                     visualLocalTransform.ValueRW.Scale = 0f;
                 } 
            }
        } 
    }
}