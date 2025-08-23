using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    public partial struct SelectVisualSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
            {
                var visualLocalTransform =  SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = 0f;
            }

            foreach (RefRO<Selected> selected in SystemAPI.Query<RefRO<Selected>>())
            {
                var visualLocalTransform =  SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
                visualLocalTransform.ValueRW.Scale = selected.ValueRO.showScale;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}