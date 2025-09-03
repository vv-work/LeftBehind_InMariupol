using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Authoring
{
    public class RandomWalkingAuthoring : MonoBehaviour
    {
        [SerializeField] 
        private float3 originPosition; 
        [SerializeField] 
        private float3 targetPosition; 
        [SerializeField] 
        private float distanceMin = 0.1f; 
        [SerializeField] 
        private float distanceMax = 10f;
        
        public uint Seed = 56;
        
        private class RandomWalkingAuthoringBaker : Baker<RandomWalkingAuthoring>
        {
            public override void Bake(RandomWalkingAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var randomWalkingData = new RandomWalkingData()
                {
                    TargetPosition = authoring.targetPosition,
                    OriginPosition = authoring.originPosition,
                    DistanceMax = authoring.distanceMax,
                    DistanceMin = authoring.distanceMin,
                    Random = new Random(authoring.Seed)

                };
                AddComponent(entity, randomWalkingData); 
            }
        } 
    }

    public struct RandomWalkingData : IComponentData
    {
        public float3 TargetPosition;
        public float3 OriginPosition;
        public float DistanceMin;
        public float DistanceMax;
        public Random Random;

    }
}