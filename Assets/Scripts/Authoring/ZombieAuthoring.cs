using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ZombieAuthoring : MonoBehaviour
    {
        private class ZombieAuthoringBaker : Baker<ZombieAuthoring>
        {
            public override void Bake(ZombieAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                ZombieData zombieData = new ZombieData();
                AddComponent(entity,zombieData);

            }
        }
    }

    public struct ZombieData : IComponentData
    {
    
    }
}