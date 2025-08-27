using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        private class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
            }
        }
    }
}