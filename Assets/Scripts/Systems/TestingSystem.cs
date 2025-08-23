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

            // int unitCount = 0;
            // foreach (var (localTransform,
            //              unitMoverData,
            //              physicsVelocity) in
            //          SystemAPI.Query< RefRW<LocalTransform>, RefRO<UnitMoverData>, RefRW<PhysicsVelocity>>()
            //              .WithDisabled<Selected>())
            // {
            //     unitCount++;
            // }
            // Debug.Log($"Unity Count {unitCount}");

        }
    }
}