using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float timerMax = 0.2f;
        [SerializeField]
        private float randomWalkingDistanceMin = .1f;
        [SerializeField]
        private float randomWalkingDistanceMax = 10f;

        private class ZombieSpawnerAuthoringBaker : Baker<ZombieSpawnerAuthoring>
        {
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var zombieSpawnerData = new ZombieSpawnerData()
                {
                    Timer = 0f,
                    TimerMax = authoring.timerMax,
                    RandomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                    RandomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                    Random = new Unity.Mathematics.Random((uint)entity.Index),
                };
                AddComponent(entity,zombieSpawnerData);
            }
        }
    }

    public struct ZombieSpawnerData : IComponentData
    {
        public float Timer, TimerMax;
        public float RandomWalkingDistanceMin, RandomWalkingDistanceMax;
        
        public Unity.Mathematics.Random Random;
    }
}