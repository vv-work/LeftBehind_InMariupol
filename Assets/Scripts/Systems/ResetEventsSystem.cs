using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ResetEventsSystem : ISystem
    
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var selected in
                     SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.OnSelected = false;
                selected.ValueRW.OnDeselected = false;
            }
            

        } 
    }
}