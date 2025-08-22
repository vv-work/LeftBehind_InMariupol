using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace Authoring
{
    public class UnitMoverAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed;

        [SerializeField]
        private float _rotationSpeed;

        private class MoveSpeedAuthoringBaker : Baker<UnitMoverAuthoring>
        {
            public override void Bake(UnitMoverAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var moveSpeedData = new UnitMoverData()
                {
                    MovementSpeed = authoring._moveSpeed,
                    RotationSpeed = authoring._rotationSpeed
                }; 
                AddComponent(entity, moveSpeedData); 
            }
        }
    }
    public struct UnitMoverData : IComponentData
    {
        public float MovementSpeed;
        public float RotationSpeed;
        public float3 TargetPosition;
    }
}