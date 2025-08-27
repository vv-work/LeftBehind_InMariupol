using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _range;

        private class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
              Entity entity  = GetEntity(TransformUsageFlags.Dynamic);
              var fiendTarget = new FindTargetData
              {
                  Range = authoring._range,
                  // TargetEntity = Entity.Null // Initialize with a default value
              };
              AddComponent(entity, fiendTarget);
            }
        }
    }

    public struct FindTargetData: IComponentData
    {
        public float Range;
        // public Entity TargetEntity;
    }
}
