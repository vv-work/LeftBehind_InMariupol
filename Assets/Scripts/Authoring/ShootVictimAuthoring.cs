using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class ShootVictimAuthoring : MonoBehaviour
    {
        [SerializeField]
        private Transform _targetPoint;

        private class ShootVictimAuthoringBaker : Baker<ShootVictimAuthoring>
        {
            public override void Bake(ShootVictimAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var shootVictim = new ShootVictim()
                { 
                    TargetPoint = authoring._targetPoint.localPosition,
                };
                AddComponent(entity,shootVictim);
                
            }
        }
    }

    public struct ShootVictim:IComponentData
    {
        public float3 TargetPoint;
    }
}