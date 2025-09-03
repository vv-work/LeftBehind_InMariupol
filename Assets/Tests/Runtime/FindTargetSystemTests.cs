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
    public class FindTargetSystemTests : EcsTestsFixture
    {
        private Entity sourceEntity;
        private Entity targetEntity;

        protected override void OnCreate()
        {
            base.OnCreate();
            World.GetOrCreateSystem<FindTargetSystem>();
        }

        [Test]
        public void FindTargetSystem_FindsCorrectTargetInRange()
        {
            // Arrange
            sourceEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            targetEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(PhysicsCollider)
            );

            // Set up source entity
            MManager.SetComponentData(sourceEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Friendly,
                Timer = 0f,
                TimerMax = 0.2f
            });

            MManager.SetComponentData(sourceEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            // Set up target entity
            MManager.SetComponentData(targetEntity, new LocalTransform
            {
                Position = new float3(5, 0, 0), // Within range
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(targetEntity, new UnitData
            {
                Faction = Faction.Friendly
            });

            // Create a simple box collider for the target
            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            
            MManager.SetComponentData(targetEntity, new PhysicsCollider { Value = boxCollider });

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var targetData = MManager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(targetEntity, targetData.TargetEntity, "Target should be found and assigned");
        }

        [Test]
        public void FindTargetSystem_DoesNotFindTargetOutOfRange()
        {
            // Arrange
            sourceEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            targetEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(PhysicsCollider)
            );

            // Set up source entity
            MManager.SetComponentData(sourceEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 5f,
                TargetFaction = Faction.Friendly,
                Timer = 0f,
                TimerMax = 0.2f
            });

            MManager.SetComponentData(sourceEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            // Set up target entity (out of range)
            MManager.SetComponentData(targetEntity, new LocalTransform
            {
                Position = new float3(20, 0, 0), // Out of range
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(targetEntity, new UnitData
            {
                Faction = Faction.Friendly
            });

            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            
            MManager.SetComponentData(targetEntity, new PhysicsCollider { Value = boxCollider });

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var targetData = MManager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(Entity.Null, targetData.TargetEntity, "Target should not be found when out of range");
        }

        [Test]
        public void FindTargetSystem_DoesNotFindWrongFaction()
        {
            // Arrange
            sourceEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            targetEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(PhysicsCollider)
            );

            // Set up source entity looking for Enemy faction
            MManager.SetComponentData(sourceEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Zombies, // Looking for Zombies
                Timer = 0f,
                TimerMax = 0.2f
            });

            MManager.SetComponentData(sourceEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            // Set up target entity as Friendly faction
            MManager.SetComponentData(targetEntity, new LocalTransform
            {
                Position = new float3(5, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            MManager.SetComponentData(targetEntity, new UnitData
            {
                Faction = Faction.Friendly // But target is Friendly
            });

            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            
            MManager.SetComponentData(targetEntity, new PhysicsCollider { Value = boxCollider });

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var targetData = MManager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(Entity.Null, targetData.TargetEntity, "Target should not be found when faction doesn't match");
        }

        [Test]
        public void FindTargetSystem_RespectsCooldownTimer()
        {
            // Arrange
            sourceEntity = MManager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            // Set up source entity with active timer
            MManager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Friendly,
                Timer = 0.1f, // Timer is active
                TimerMax = 0.2f
            });

            var initialTargetData = new TargetData { TargetEntity = Entity.Null };
            MManager.SetComponentData(sourceEntity, initialTargetData);

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var findTargetData = MManager.GetComponentData<FindTargetData>(sourceEntity);
            Assert.Less(findTargetData.Timer, 0.1f, "Timer should decrease");
            
            var targetData = MManager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(Entity.Null, targetData.TargetEntity, "Should not search for target while timer is active");
        }
    }
}