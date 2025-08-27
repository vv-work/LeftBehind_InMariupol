using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct TestingSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            int unitCount = 0;
            foreach (var firendly in
                     SystemAPI.Query<RefRO<FriendlyData>>()) {
                unitCount++;
            }
            Debug.Log($"Unit Count {unitCount}");

        }
    }
}