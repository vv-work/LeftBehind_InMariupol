using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TargetAuthoring : MonoBehaviour
    {
        private class TargetAuthoringBaker : Baker<TargetAuthoring>
        {
            public override void Bake(TargetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<TargetData>(entity, new TargetData());
            }
        }
    }
    
    public struct TargetData : IComponentData
    {
        public Entity TargetEntity;
    }
}