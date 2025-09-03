using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float timerMax = 0.2f;

        private class ZombieSpawnerAuthoringBaker : Baker<ZombieSpawnerAuthoring>
        {
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var zombieSpawnerData = new ZombieSpawnerData()
                {
                    Timer = 0f,
                    TimerMax = authoring.timerMax,
                };
                AddComponent(entity,zombieSpawnerData);
            }
        }
    }

    public struct ZombieSpawnerData : IComponentData
    {
        public float Timer;
        public float TimerMax;
    }
}