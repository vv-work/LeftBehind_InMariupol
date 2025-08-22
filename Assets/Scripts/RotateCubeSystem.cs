using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class RotateCubeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (localTransform, rotationSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
        {
            quaternion q = quaternion.RotateY(rotationSpeed.ValueRO.Value * deltaTime);
            localTransform.ValueRW.Rotation = math.mul(localTransform.ValueRO.Rotation, q);
        }
    }
}
