using Unity.Entities;
using UnityEngine;

public class RotationSpeedAuthoring : MonoBehaviour
{
    [SerializeField]
    private float rotSpeed = 10f;

    class Baker : Baker<RotationSpeedAuthoring>
    {
        public override void Bake(RotationSpeedAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RotationSpeed { 
                Value = authoring.rotSpeed 
                });
        }
    }
}

public struct RotationSpeed : IComponentData
{
    public float Value;
}
