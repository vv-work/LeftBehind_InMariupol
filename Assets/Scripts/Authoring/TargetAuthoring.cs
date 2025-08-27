using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TargetAuthoring : MonoBehaviour
    {
        [SerializeField]
        private GameObject _target;

        private class TargetAuthoringBaker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var targetData = new TargetData() {
                    TargetEntity = GetEntity(authoring._target, TransformUsageFlags.Dynamic)
                };
                AddComponent<TargetData>(entity, targetData);
            }
        }
    }
    
    public struct TargetData : IComponentData
    {
        public Entity TargetEntity;
    }
}