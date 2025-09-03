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
                var targetPoint = authoring._targetPoint != null 
                    ? (float3)authoring._targetPoint.localPosition 
                    : float3.zero;
                    
                var shootVictim = new ShootVictim()
                { 
                    TargetPoint = targetPoint,
                };
                AddComponent(entity, shootVictim);
            }
        }
    }

    public struct ShootVictim:IComponentData
    {
        public float3 TargetPoint;
    }
}