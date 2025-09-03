using System.Collections;
using Authoring;
using MonoBehaviours;
using NUnit.Framework;
using Systems;
using Tests.Runtime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [TestFixture]
    public class IntegrationTests : EcsTestsFixture
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            World.GetOrCreateSystem<FindTargetSystem>();
            World.GetOrCreateSystem<UnitMoverSystem>();
            World.GetOrCreateSystem<SelectVisualSystem>();
            World.GetOrCreateSystem<ResetEventsSystem>();
        }

        [Test]
        public void IntegrationTest_CompleteUnitWorkflow()
        {
            // Arrange - Create a complete unit with all systems
            var unitEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(UnitMoverData),
                typeof(FindTargetData),
                typeof(TargetData),
                typeof(PhysicsVelocity),
                typeof(PhysicsCollider),
                typeof(Selected)
            );

            var visualEntity = MManager.CreateEntity(typeof(LocalTransform));

            // Set up the unit
            MManager.SetComponentData(unitEntity, new LocalTransform
            {
                Position = new float3(0, 0.1f, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(unitEntity, new UnitData
            {
                Faction = Faction.Friendly
            });

            MManager.SetComponentData(unitEntity, new UnitMoverData
            {
                MovementSpeed = 5f,
                RotationSpeed = 2f,
                TargetPosition = new float3(10, 0.1f, 0)
            });

            MManager.SetComponentData(unitEntity, new FindTargetData
            {
                Range = 8f,
                TargetFaction = Faction.Zombies,
                Timer = 0f,
                TimerMax = 0.2f
            });

            MManager.SetComponentData(unitEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            MManager.SetComponentData(unitEntity, new PhysicsVelocity
            {
                Linear = float3.zero,
                Angular = float3.zero
            });

            // Create collider for the unit
            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            MManager.SetComponentData(unitEntity, new PhysicsCollider { Value = boxCollider });

            MManager.SetComponentData(unitEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 1.5f,
                OnSelected = true,
                OnDeselected = false
            });

            MManager.SetComponentData(visualEntity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 0f
            });

            MManager.SetComponentEnabled<Selected>(unitEntity, true);

            // Act - Run all systems in order
            UpdateUnitMoverSystem();
            UpdateSelectVisualSystem();
            UpdateResetEventsSystem();

            // Assert - Verify all systems worked together
            var transform = MManager.GetComponentData<LocalTransform>(unitEntity);
            var velocity = MManager.GetComponentData<PhysicsVelocity>(unitEntity);
            var selected = MManager.GetComponentData<Selected>(unitEntity);
            var visualTransform = MManager.GetComponentData<LocalTransform>(visualEntity);

            // Movement system should have set velocity
            Assert.Greater(velocity.Linear.x, 0, "Unit should be moving towards target");

            // Selection visual should be shown
            Assert.AreEqual(1.5f, visualTransform.Scale, 0.001f, "Selection visual should be visible");

            // Events should be reset
            Assert.IsFalse(selected.OnSelected, "Selection event should be reset");
            Assert.IsFalse(selected.OnDeselected, "Deselection event should remain false");
        }

        [Test]
        public void IntegrationTest_MultipleUnitsInteraction()
        {
            // Arrange - Create two units that could interact
            var friendlyUnit = CreateTestUnit(new float3(0, 0.1f, 0), Faction.Friendly);
            var enemyUnit = CreateTestUnit(new float3(5, 0.1f, 0), Faction.Zombies);

            // Make friendly unit search for zombies
            MManager.SetComponentData(friendlyUnit, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Zombies,
                Timer = 0f,
                TimerMax = 0.2f
            });

            // Act
            UpdatePhysicsWorld(); // Update physics for collision detection
            UpdateFindTargetSystem(); // Should find enemy target
            UpdateUnitMoverSystem(); // Should move units

            // Assert
            var targetData = MManager.GetComponentData<TargetData>(friendlyUnit);
            var friendlyVelocity = MManager.GetComponentData<PhysicsVelocity>(friendlyUnit);
            var enemyVelocity = MManager.GetComponentData<PhysicsVelocity>(enemyUnit);

            Assert.AreEqual(enemyUnit, targetData.TargetEntity, "Friendly unit should target enemy unit");
            Assert.Greater(math.length(friendlyVelocity.Linear), 0, "Friendly unit should be moving");
            Assert.Greater(math.length(enemyVelocity.Linear), 0, "Enemy unit should be moving to its own target");
        }

        [UnityTest]
        public IEnumerator IntegrationTest_SystemsWorkOverTime()
        {
            // Arrange
            var unit = CreateTestUnit(new float3(0, 0.1f, 0), Faction.Friendly);
            
            var initialPosition = MManager.GetComponentData<LocalTransform>(unit).Position;
            var targetPosition = new float3(5, 0.1f, 0);

            MManager.SetComponentData(unit, new UnitMoverData
            {
                MovementSpeed = 10f,
                RotationSpeed = 5f,
                TargetPosition = targetPosition
            });

            // Act & Assert - Run systems over multiple frames
            for (int frame = 0; frame < 10; frame++)
            {
                UpdateUnitMoverSystem();
                yield return null; // Wait one frame
            }

            var finalPosition = MManager.GetComponentData<LocalTransform>(unit).Position;
            var distanceMoved = math.distance(initialPosition, finalPosition);

            Assert.Greater(distanceMoved, 0, "Unit should have moved over time");
        }

        private Entity CreateTestUnit(float3 position, Faction faction)
        {
            var unit = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(UnitMoverData),
                typeof(FindTargetData),
                typeof(TargetData),
                typeof(PhysicsVelocity),
                typeof(PhysicsCollider)
            );

            MManager.SetComponentData(unit, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(unit, new UnitData { Faction = faction });

            MManager.SetComponentData(unit, new UnitMoverData
            {
                MovementSpeed = 5f,
                RotationSpeed = 2f,
                TargetPosition = position + new float3(10, 0, 0)
            });

            MManager.SetComponentData(unit, new FindTargetData
            {
                Range = 8f,
                TargetFaction = faction == Faction.Friendly ? Faction.Zombies : Faction.Friendly,
                Timer = 0f,
                TimerMax = 0.2f
            });

            MManager.SetComponentData(unit, new TargetData { TargetEntity = Entity.Null });
            MManager.SetComponentData(unit, new PhysicsVelocity());

            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            MManager.SetComponentData(unit, new PhysicsCollider { Value = boxCollider });

            return unit;
        }

        private void UpdateUnitMoverSystem()
        {
            var systemHandle = World.GetExistingSystem<UnitMoverSystem>();
            systemHandle.Update(World.Unmanaged);
        }

        private void UpdateSelectVisualSystem()
        {
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);
        }

        private void UpdateResetEventsSystem()
        {
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);
        }

        private void UpdateFindTargetSystem()
        {
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);
        }
    }
}