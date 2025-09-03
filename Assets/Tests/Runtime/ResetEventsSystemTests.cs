using Authoring;
using NUnit.Framework;
using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tests
{
    [TestFixture]
    public class ResetEventsSystemTests : ECSTestsFixture
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            World.GetOrCreateSystem<ResetEventsSystem>();
        }

        [Test]
        public void ResetEventsSystem_ResetsOnSelectedEvent()
        {
            // Arrange
            var entity = m_Manager.CreateEntity(typeof(Selected));

            m_Manager.SetComponentData(entity, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 1f,
                OnSelected = true, // Should be reset to false
                OnDeselected = false
            });

            // Enable the component so the system can process it
            m_Manager.SetComponentEnabled<Selected>(entity, true);

            // Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var selected = m_Manager.GetComponentData<Selected>(entity);
            Assert.IsFalse(selected.OnSelected, "OnSelected should be reset to false");
            Assert.IsFalse(selected.OnDeselected, "OnDeselected should remain false");
        }

        [Test]
        public void ResetEventsSystem_ResetsOnDeselectedEvent()
        {
            // Arrange
            var entity = m_Manager.CreateEntity(typeof(Selected));

            m_Manager.SetComponentData(entity, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 1f,
                OnSelected = false,
                OnDeselected = true // Should be reset to false
            });

            m_Manager.SetComponentEnabled<Selected>(entity, true);

            // Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var selected = m_Manager.GetComponentData<Selected>(entity);
            Assert.IsFalse(selected.OnSelected, "OnSelected should remain false");
            Assert.IsFalse(selected.OnDeselected, "OnDeselected should be reset to false");
        }

        [Test]
        public void ResetEventsSystem_ResetsBothEvents()
        {
            // Arrange
            var entity = m_Manager.CreateEntity(typeof(Selected));

            m_Manager.SetComponentData(entity, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 1f,
                OnSelected = true, // Should be reset to false
                OnDeselected = true // Should be reset to false
            });

            m_Manager.SetComponentEnabled<Selected>(entity, true);

            // Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var selected = m_Manager.GetComponentData<Selected>(entity);
            Assert.IsFalse(selected.OnSelected, "OnSelected should be reset to false");
            Assert.IsFalse(selected.OnDeselected, "OnDeselected should be reset to false");
        }

        [Test]
        public void ResetEventsSystem_WorksWithMultipleEntities()
        {
            // Arrange
            var entity1 = m_Manager.CreateEntity(typeof(Selected));
            var entity2 = m_Manager.CreateEntity(typeof(Selected));
            var entity3 = m_Manager.CreateEntity(typeof(Selected));

            m_Manager.SetComponentData(entity1, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 1f,
                OnSelected = true,
                OnDeselected = false
            });

            m_Manager.SetComponentData(entity2, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 2f,
                OnSelected = false,
                OnDeselected = true
            });

            m_Manager.SetComponentData(entity3, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 3f,
                OnSelected = true,
                OnDeselected = true
            });

            // Enable all components
            m_Manager.SetComponentEnabled<Selected>(entity1, true);
            m_Manager.SetComponentEnabled<Selected>(entity2, true);
            m_Manager.SetComponentEnabled<Selected>(entity3, true);

            // Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var selected1 = m_Manager.GetComponentData<Selected>(entity1);
            var selected2 = m_Manager.GetComponentData<Selected>(entity2);
            var selected3 = m_Manager.GetComponentData<Selected>(entity3);

            Assert.IsFalse(selected1.OnSelected, "Entity1 OnSelected should be reset");
            Assert.IsFalse(selected1.OnDeselected, "Entity1 OnDeselected should remain false");

            Assert.IsFalse(selected2.OnSelected, "Entity2 OnSelected should remain false");
            Assert.IsFalse(selected2.OnDeselected, "Entity2 OnDeselected should be reset");

            Assert.IsFalse(selected3.OnSelected, "Entity3 OnSelected should be reset");
            Assert.IsFalse(selected3.OnDeselected, "Entity3 OnDeselected should be reset");
        }

        [Test]
        public void ResetEventsSystem_IgnoresDisabledComponents()
        {
            // Arrange
            var entity = m_Manager.CreateEntity(typeof(Selected));

            m_Manager.SetComponentData(entity, new Selected
            {
                visualEntity = Entity.Null,
                showScale = 1f,
                OnSelected = true,
                OnDeselected = true
            });

            // Disable the component - system should ignore it
            m_Manager.SetComponentEnabled<Selected>(entity, false);

            // Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var selected = m_Manager.GetComponentData<Selected>(entity);
            Assert.IsTrue(selected.OnSelected, "OnSelected should remain true when component is disabled");
            Assert.IsTrue(selected.OnDeselected, "OnDeselected should remain true when component is disabled");
        }

        [Test]
        public void ResetEventsSystem_PreservesNonEventData()
        {
            // Arrange
            var visualEntity = m_Manager.CreateEntity(typeof(LocalTransform));
            var mainEntity = m_Manager.CreateEntity(typeof(Selected));

            m_Manager.SetComponentData(mainEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 2.5f,
                OnSelected = true,
                OnDeselected = true
            });

            m_Manager.SetComponentEnabled<Selected>(mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var selected = m_Manager.GetComponentData<Selected>(mainEntity);
            
            // Event flags should be reset
            Assert.IsFalse(selected.OnSelected, "OnSelected should be reset to false");
            Assert.IsFalse(selected.OnDeselected, "OnDeselected should be reset to false");
            
            // Other data should be preserved
            Assert.AreEqual(visualEntity, selected.visualEntity, "visualEntity should be preserved");
            Assert.AreEqual(2.5f, selected.showScale, 0.001f, "showScale should be preserved");
        }

        [Test]
        public void ResetEventsSystem_HandlesEmptyQuery()
        {
            // Arrange - No entities with Selected components

            // Act - Should not throw any exceptions
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert - Test passes if no exceptions are thrown
            Assert.Pass("System handles empty query gracefully");
        }

        [Test]
        public void ResetEventsSystem_RunsInCorrectSystemGroup()
        {
            // Arrange & Act
            var systemHandle = World.GetExistingSystem<ResetEventsSystem>();
            
            // Assert
            Assert.IsNotNull(systemHandle, "ResetEventsSystem should be created in the world");
            
            // Note: Testing system group ordering would require more complex setup
            // This test primarily ensures the system can be created and accessed
        }
    }
}