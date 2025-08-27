using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FriendlyAuthoring : MonoBehaviour
    {
        private class FriendlyAuthoringBaker : Baker<FriendlyAuthoring>
        {
            public override void Bake(FriendlyAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var friendly = new FriendlyData();
                AddComponent(entity,friendly);
            }
        }
    }

    public struct FriendlyData : IComponentData
    {
        
    }
}