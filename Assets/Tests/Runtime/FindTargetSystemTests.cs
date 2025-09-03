using System.Collections;
using Authoring;
using MonoBehaviours;
using NUnit.Framework;
using Systems;
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
    public class FindTargetSystemTests : ECSTestsFixture
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
            sourceEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            targetEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(PhysicsCollider)
            );

            // Set up source entity
            m_Manager.SetComponentData(sourceEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            m_Manager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Friendly,
                Timer = 0f,
                TimerMax = 0.2f
            });

            m_Manager.SetComponentData(sourceEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            // Set up target entity
            m_Manager.SetComponentData(targetEntity, new LocalTransform
            {
                Position = new float3(5, 0, 0), // Within range
                Rotation = quaternion.identity,
                Scale = 1
            });

            m_Manager.SetComponentData(targetEntity, new UnitData
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
            
            m_Manager.SetComponentData(targetEntity, new PhysicsCollider { Value = boxCollider });

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var targetData = m_Manager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(targetEntity, targetData.TargetEntity, "Target should be found and assigned");
        }

        [Test]
        public void FindTargetSystem_DoesNotFindTargetOutOfRange()
        {
            // Arrange
            sourceEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            targetEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(PhysicsCollider)
            );

            // Set up source entity
            m_Manager.SetComponentData(sourceEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            m_Manager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 5f,
                TargetFaction = Faction.Friendly,
                Timer = 0f,
                TimerMax = 0.2f
            });

            m_Manager.SetComponentData(sourceEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            // Set up target entity (out of range)
            m_Manager.SetComponentData(targetEntity, new LocalTransform
            {
                Position = new float3(20, 0, 0), // Out of range
                Rotation = quaternion.identity,
                Scale = 1
            });

            m_Manager.SetComponentData(targetEntity, new UnitData
            {
                Faction = Faction.Friendly
            });

            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            
            m_Manager.SetComponentData(targetEntity, new PhysicsCollider { Value = boxCollider });

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var targetData = m_Manager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(Entity.Null, targetData.TargetEntity, "Target should not be found when out of range");
        }

        [Test]
        public void FindTargetSystem_DoesNotFindWrongFaction()
        {
            // Arrange
            sourceEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            targetEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(UnitData),
                typeof(PhysicsCollider)
            );

            // Set up source entity looking for Enemy faction
            m_Manager.SetComponentData(sourceEntity, new LocalTransform
            {
                Position = new float3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            m_Manager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Zombies, // Looking for Zombies
                Timer = 0f,
                TimerMax = 0.2f
            });

            m_Manager.SetComponentData(sourceEntity, new TargetData
            {
                TargetEntity = Entity.Null
            });

            // Set up target entity as Friendly faction
            m_Manager.SetComponentData(targetEntity, new LocalTransform
            {
                Position = new float3(5, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1
            });

            m_Manager.SetComponentData(targetEntity, new UnitData
            {
                Faction = Faction.Friendly // But target is Friendly
            });

            var boxCollider = Unity.Physics.BoxCollider.Create(new BoxGeometry
            {
                Center = float3.zero,
                Orientation = quaternion.identity,
                Size = new float3(1, 1, 1)
            });
            
            m_Manager.SetComponentData(targetEntity, new PhysicsCollider { Value = boxCollider });

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var targetData = m_Manager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(Entity.Null, targetData.TargetEntity, "Target should not be found when faction doesn't match");
        }

        [Test]
        public void FindTargetSystem_RespectsCooldownTimer()
        {
            // Arrange
            sourceEntity = m_Manager.CreateEntity(
                typeof(LocalTransform),
                typeof(FindTargetData),
                typeof(TargetData)
            );

            // Set up source entity with active timer
            m_Manager.SetComponentData(sourceEntity, new FindTargetData
            {
                Range = 10f,
                TargetFaction = Faction.Friendly,
                Timer = 0.1f, // Timer is active
                TimerMax = 0.2f
            });

            var initialTargetData = new TargetData { TargetEntity = Entity.Null };
            m_Manager.SetComponentData(sourceEntity, initialTargetData);

            // Act
            var systemHandle = World.GetExistingSystem<FindTargetSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var findTargetData = m_Manager.GetComponentData<FindTargetData>(sourceEntity);
            Assert.Less(findTargetData.Timer, 0.1f, "Timer should decrease");
            
            var targetData = m_Manager.GetComponentData<TargetData>(sourceEntity);
            Assert.AreEqual(Entity.Null, targetData.TargetEntity, "Should not search for target while timer is active");
        }
    }
}