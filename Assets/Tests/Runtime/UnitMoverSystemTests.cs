using Authoring;
using NUnit.Framework;
using Systems;
using Tests.Runtime;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class UnitMoverSystemTests : EcsTestsFixture
    {
        private Entity moverEntity;

        protected override void OnCreate()
        {
            base.OnCreate();
            World.GetOrCreateSystem<UnitMoverSystem>();
        }

        [Test]
        public void UnitMoverSystem_MovesTowardsTarget()
        {
            // Arrange
            moverEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitMoverData),
                typeof(PhysicsVelocity)
            );

            var startPosition = new float3(0, 0.1f, 0);
            var targetPosition = new float3(10, 0.1f, 0);

            MManager.SetComponentData(moverEntity, new LocalTransform
            {
                Position = startPosition,
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(moverEntity, new UnitMoverData
            {
                MovementSpeed = 5f,
                RotationSpeed = 2f,
                TargetPosition = targetPosition
            });

            MManager.SetComponentData(moverEntity, new PhysicsVelocity
            {
                Linear = float3.zero,
                Angular = float3.zero
            });

            // Act
            var systemHandle = World.GetExistingSystem<UnitMoverSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var physicsVelocity = MManager.GetComponentData<PhysicsVelocity>(moverEntity);
            var localTransform = MManager.GetComponentData<LocalTransform>(moverEntity);

            // Should have velocity in the direction of target
            Assert.Greater(physicsVelocity.Linear.x, 0, "Should have positive X velocity towards target");
            Assert.AreEqual(0, physicsVelocity.Angular.x, 0.001f, "Angular velocity should be zeroed");
            Assert.AreEqual(0, physicsVelocity.Angular.y, 0.001f, "Angular velocity should be zeroed");
            Assert.AreEqual(0, physicsVelocity.Angular.z, 0.001f, "Angular velocity should be zeroed");
            
            // Y position should be locked to 0.1f
            Assert.AreEqual(0.1f, localTransform.Position.y, 0.001f, "Y position should be locked to 0.1f");
        }

        [Test]
        public void UnitMoverSystem_StopsWhenNearTarget()
        {
            // Arrange
            moverEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitMoverData),
                typeof(PhysicsVelocity)
            );

            var nearTargetPosition = new float3(0, 0.1f, 0);
            var targetPosition = new float3(0.5f, 0.1f, 0); // Close to target (within 0.5f threshold)

            MManager.SetComponentData(moverEntity, new LocalTransform
            {
                Position = nearTargetPosition,
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(moverEntity, new UnitMoverData
            {
                MovementSpeed = 5f,
                RotationSpeed = 2f,
                TargetPosition = targetPosition
            });

            MManager.SetComponentData(moverEntity, new PhysicsVelocity
            {
                Linear = new float3(1, 0, 0), // Initial velocity
                Angular = new float3(0, 1, 0) // Initial angular velocity
            });

            // Act
            var systemHandle = World.GetExistingSystem<UnitMoverSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var physicsVelocity = MManager.GetComponentData<PhysicsVelocity>(moverEntity);

            Assert.AreEqual(float3.zero, physicsVelocity.Linear, "Linear velocity should be zero when near target");
            Assert.AreEqual(float3.zero, physicsVelocity.Angular, "Angular velocity should be zero when near target");
        }

        [Test]
        public void UnitMoverSystem_RotatesTowardsMovementDirection()
        {
            // Arrange
            moverEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitMoverData),
                typeof(PhysicsVelocity)
            );

            var startPosition = new float3(0, 0.1f, 0);
            var targetPosition = new float3(0, 0.1f, 10); // Target in Z direction

            MManager.SetComponentData(moverEntity, new LocalTransform
            {
                Position = startPosition,
                Rotation = quaternion.identity, // Facing forward initially
                Scale = 1
            });

            MManager.SetComponentData(moverEntity, new UnitMoverData
            {
                MovementSpeed = 5f,
                RotationSpeed = 2f,
                TargetPosition = targetPosition
            });

            MManager.SetComponentData(moverEntity, new PhysicsVelocity
            {
                Linear = float3.zero,
                Angular = float3.zero
            });

            var initialRotation = MManager.GetComponentData<LocalTransform>(moverEntity).Rotation;

            // Act
            var systemHandle = World.GetExistingSystem<UnitMoverSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var finalTransform = MManager.GetComponentData<LocalTransform>(moverEntity);
            
            // The rotation should have changed to face the movement direction
            Assert.AreNotEqual(initialRotation, finalTransform.Rotation, "Rotation should change to face target");
            
            // Should have velocity towards target
            var physicsVelocity = MManager.GetComponentData<PhysicsVelocity>(moverEntity);
            Assert.Greater(physicsVelocity.Linear.z, 0, "Should have positive Z velocity towards target");
        }

        [Test]
        public void UnitMoverSystem_WorksWithMultipleEntities()
        {
            // Arrange
            var entity1 = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitMoverData),
                typeof(PhysicsVelocity)
            );

            var entity2 = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitMoverData),
                typeof(PhysicsVelocity)
            );

            // Set up entity 1
            MManager.SetComponentData(entity1, new LocalTransform
            {
                Position = new float3(0, 0.1f, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(entity1, new UnitMoverData
            {
                MovementSpeed = 3f,
                RotationSpeed = 1f,
                TargetPosition = new float3(5, 0.1f, 0)
            });

            // Set up entity 2
            MManager.SetComponentData(entity2, new LocalTransform
            {
                Position = new float3(10, 0.1f, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(entity2, new UnitMoverData
            {
                MovementSpeed = 7f,
                RotationSpeed = 3f,
                TargetPosition = new float3(15, 0.1f, 0)
            });

            // Act
            var systemHandle = World.GetExistingSystem<UnitMoverSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var velocity1 = MManager.GetComponentData<PhysicsVelocity>(entity1);
            var velocity2 = MManager.GetComponentData<PhysicsVelocity>(entity2);

            Assert.Greater(velocity1.Linear.x, 0, "Entity 1 should move towards target");
            Assert.Greater(velocity2.Linear.x, 0, "Entity 2 should move towards target");
            
            // Entity 2 should have higher velocity due to higher movement speed
            Assert.Greater(math.length(velocity2.Linear), math.length(velocity1.Linear), 
                "Entity 2 with higher movement speed should have higher velocity");
        }

        [Test]
        public void UnitMoverSystem_HandlesZeroMovementSpeed()
        {
            // Arrange
            moverEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitMoverData),
                typeof(PhysicsVelocity)
            );

            MManager.SetComponentData(moverEntity, new LocalTransform
            {
                Position = new float3(0, 0.1f, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(moverEntity, new UnitMoverData
            {
                MovementSpeed = 0f, // Zero speed
                RotationSpeed = 2f,
                TargetPosition = new float3(10, 0.1f, 0)
            });

            MManager.SetComponentData(moverEntity, new PhysicsVelocity
            {
                Linear = float3.zero,
                Angular = float3.zero
            });

            // Act
            var systemHandle = World.GetExistingSystem<UnitMoverSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var physicsVelocity = MManager.GetComponentData<PhysicsVelocity>(moverEntity);
            
            Assert.AreEqual(0, physicsVelocity.Linear.x, 0.001f, "Should not move with zero movement speed");
            Assert.AreEqual(0, physicsVelocity.Linear.z, 0.001f, "Should not move with zero movement speed");
        }
    }
}