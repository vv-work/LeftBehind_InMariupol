using Authoring;
using NUnit.Framework;
using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tests
{
    [TestFixture]
    public class SelectVisualSystemTests : ECSTestsFixture
    {
        private Entity mainEntity;
        private Entity visualEntity;

        protected override void OnCreate()
        {
            base.OnCreate();
            World.GetOrCreateSystem<SelectVisualSystem>();
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();

            // Create entities for each test
            mainEntity = m_Manager.CreateEntity(typeof(Selected));
            visualEntity = m_Manager.CreateEntity(typeof(LocalTransform));

            // Set up visual entity
            m_Manager.SetComponentData(visualEntity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 0f // Initially hidden
            });
        }

        [Test]
        public void SelectVisualSystem_ShowsVisualOnSelection()
        {
            // Arrange
            m_Manager.SetComponentData(mainEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 1.5f,
                OnSelected = true, // Trigger selection event
                OnDeselected = false
            });

            // Enable the Selected component
            m_Manager.SetComponentEnabled<Selected>(mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = m_Manager.GetComponentData<LocalTransform>(visualEntity);
            Assert.AreEqual(1.5f, visualTransform.Scale, 0.001f, "Visual should be scaled to showScale when selected");
        }

        [Test]
        public void SelectVisualSystem_HidesVisualOnDeselection()
        {
            // Arrange
            // First set the visual to be visible
            m_Manager.SetComponentData(visualEntity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1.5f // Initially visible
            });

            m_Manager.SetComponentData(mainEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 1.5f,
                OnSelected = false,
                OnDeselected = true // Trigger deselection event
            });

            // Enable the Selected component
            m_Manager.SetComponentEnabled<Selected>(mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = m_Manager.GetComponentData<LocalTransform>(visualEntity);
            Assert.AreEqual(0f, visualTransform.Scale, 0.001f, "Visual should be hidden (scale 0) when deselected");
        }

        [Test]
        public void SelectVisualSystem_HandlesMultipleSelections()
        {
            // Arrange
            var entity2 = m_Manager.CreateEntity(typeof(Selected));
            var visualEntity2 = m_Manager.CreateEntity(typeof(LocalTransform));

            m_Manager.SetComponentData(visualEntity2, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 0f
            });

            // Set up first entity for selection
            m_Manager.SetComponentData(mainEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 2f,
                OnSelected = true,
                OnDeselected = false
            });
            m_Manager.SetComponentEnabled<Selected>(mainEntity, true);

            // Set up second entity for deselection
            m_Manager.SetComponentData(entity2, new Selected
            {
                visualEntity = visualEntity2,
                showScale = 3f,
                OnSelected = false,
                OnDeselected = true
            });
            m_Manager.SetComponentEnabled<Selected>(entity2, true);

            // Initially make second visual visible
            m_Manager.SetComponentData(visualEntity2, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 3f
            });

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visual1Transform = m_Manager.GetComponentData<LocalTransform>(visualEntity);
            var visual2Transform = m_Manager.GetComponentData<LocalTransform>(visualEntity2);

            Assert.AreEqual(2f, visual1Transform.Scale, 0.001f, "First visual should be selected (scale 2)");
            Assert.AreEqual(0f, visual2Transform.Scale, 0.001f, "Second visual should be deselected (scale 0)");
        }

        [Test]
        public void SelectVisualSystem_IgnoresDisabledComponents()
        {
            // Arrange
            m_Manager.SetComponentData(mainEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 1.5f,
                OnSelected = true,
                OnDeselected = false
            });

            // Disable the Selected component - system should ignore it
            m_Manager.SetComponentEnabled<Selected>(mainEntity, false);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = m_Manager.GetComponentData<LocalTransform>(visualEntity);
            Assert.AreEqual(0f, visualTransform.Scale, 0.001f, "Visual should remain unchanged when component is disabled");
        }

        [Test]
        public void SelectVisualSystem_HandlesBothEventsAtOnce()
        {
            // Arrange - Both selection and deselection events are true (edge case)
            m_Manager.SetComponentData(mainEntity, new Selected
            {
                visualEntity = visualEntity,
                showScale = 1.5f,
                OnSelected = true,
                OnDeselected = true // Both events active
            });

            m_Manager.SetComponentEnabled<Selected>(mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = m_Manager.GetComponentData<LocalTransform>(visualEntity);
            // Since deselection is processed first, then selection, the final result should be selected
            Assert.AreEqual(1.5f, visualTransform.Scale, 0.001f, "When both events are active, selection should take precedence");
        }

        [Test]
        public void SelectVisualSystem_WorksWithDifferentShowScales()
        {
            // Arrange
            var testScales = new float[] { 0.5f, 1f, 2f, 5f };

            foreach (var scale in testScales)
            {
                m_Manager.SetComponentData(mainEntity, new Selected
                {
                    visualEntity = visualEntity,
                    showScale = scale,
                    OnSelected = true,
                    OnDeselected = false
                });

                m_Manager.SetComponentEnabled<Selected>(mainEntity, true);

                // Act
                var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
                systemHandle.Update(World.Unmanaged);

                // Assert
                var visualTransform = m_Manager.GetComponentData<LocalTransform>(visualEntity);
                Assert.AreEqual(scale, visualTransform.Scale, 0.001f, $"Visual should be scaled to {scale} when selected");

                // Reset for next iteration
                m_Manager.SetComponentData(mainEntity, new Selected
                {
                    visualEntity = visualEntity,
                    showScale = scale,
                    OnSelected = false,
                    OnDeselected = true
                });

                systemHandle.Update(World.Unmanaged);
            }
        }
    }
}