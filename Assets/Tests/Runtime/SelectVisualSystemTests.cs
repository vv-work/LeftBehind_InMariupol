using Authoring;
using NUnit.Framework;
using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tests.Runtime
{
    [TestFixture]
    public class SelectVisualSystemTests : EcsTestsFixture
    {
        private Entity _mainEntity;
        private Entity _visualEntity;

        protected override void OnCreate()
        {
            base.OnCreate();
            World.GetOrCreateSystem<SelectVisualSystem>();
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();

            // Create entities for each test
            _mainEntity = MManager.CreateEntity(typeof(Selected));
            _visualEntity = MManager.CreateEntity(typeof(LocalTransform));

            // Set up visual entity
            MManager.SetComponentData(_visualEntity, new LocalTransform
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
            MManager.SetComponentData(_mainEntity, new Selected
            {
                visualEntity = _visualEntity,
                showScale = 1.5f,
                OnSelected = true, // Trigger selection event
                OnDeselected = false
            });

            // Enable the Selected component
            MManager.SetComponentEnabled<Selected>(_mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = MManager.GetComponentData<LocalTransform>(_visualEntity);
            Assert.AreEqual(1.5f, visualTransform.Scale, 0.001f, "Visual should be scaled to showScale when selected");
        }

        [Test]
        public void SelectVisualSystem_HidesVisualOnDeselection()
        {
            // Arrange
            // First set the visual to be visible
            MManager.SetComponentData(_visualEntity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1.5f // Initially visible
            });

            MManager.SetComponentData(_mainEntity, new Selected
            {
                visualEntity = _visualEntity,
                showScale = 1.5f,
                OnSelected = false,
                OnDeselected = true // Trigger deselection event
            });

            // Enable the Selected component
            MManager.SetComponentEnabled<Selected>(_mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = MManager.GetComponentData<LocalTransform>(_visualEntity);
            Assert.AreEqual(0f, visualTransform.Scale, 0.001f, "Visual should be hidden (scale 0) when deselected");
        }

        [Test]
        public void SelectVisualSystem_HandlesMultipleSelections()
        {
            // Arrange
            var entity2 = MManager.CreateEntity(typeof(Selected));
            var visualEntity2 = MManager.CreateEntity(typeof(LocalTransform));

            MManager.SetComponentData(visualEntity2, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 0f
            });

            // Set up first entity for selection
            MManager.SetComponentData(_mainEntity, new Selected
            {
                visualEntity = _visualEntity,
                showScale = 2f,
                OnSelected = true,
                OnDeselected = false
            });
            MManager.SetComponentEnabled<Selected>(_mainEntity, true);

            // Set up second entity for deselection
            MManager.SetComponentData(entity2, new Selected
            {
                visualEntity = visualEntity2,
                showScale = 3f,
                OnSelected = false,
                OnDeselected = true
            });
            MManager.SetComponentEnabled<Selected>(entity2, true);

            // Initially make second visual visible
            MManager.SetComponentData(visualEntity2, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 3f
            });

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visual1Transform = MManager.GetComponentData<LocalTransform>(_visualEntity);
            var visual2Transform = MManager.GetComponentData<LocalTransform>(visualEntity2);

            Assert.AreEqual(2f, visual1Transform.Scale, 0.001f, "First visual should be selected (scale 2)");
            Assert.AreEqual(0f, visual2Transform.Scale, 0.001f, "Second visual should be deselected (scale 0)");
        }

        [Test]
        public void SelectVisualSystem_IgnoresDisabledComponents()
        {
            // Arrange
            MManager.SetComponentData(_mainEntity, new Selected
            {
                visualEntity = _visualEntity,
                showScale = 1.5f,
                OnSelected = true,
                OnDeselected = false
            });

            // Disable the Selected component - system should ignore it
            MManager.SetComponentEnabled<Selected>(_mainEntity, false);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = MManager.GetComponentData<LocalTransform>(_visualEntity);
            Assert.AreEqual(0f, visualTransform.Scale, 0.001f, "Visual should remain unchanged when component is disabled");
        }

        [Test]
        public void SelectVisualSystem_HandlesBothEventsAtOnce()
        {
            // Arrange - Both selection and deselection events are true (edge case)
            MManager.SetComponentData(_mainEntity, new Selected
            {
                visualEntity = _visualEntity,
                showScale = 1.5f,
                OnSelected = true,
                OnDeselected = true // Both events active
            });

            MManager.SetComponentEnabled<Selected>(_mainEntity, true);

            // Act
            var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
            systemHandle.Update(World.Unmanaged);

            // Assert
            var visualTransform = MManager.GetComponentData<LocalTransform>(_visualEntity);
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
                MManager.SetComponentData(_mainEntity, new Selected
                {
                    visualEntity = _visualEntity,
                    showScale = scale,
                    OnSelected = true,
                    OnDeselected = false
                });

                MManager.SetComponentEnabled<Selected>(_mainEntity, true);

                // Act
                var systemHandle = World.GetExistingSystem<SelectVisualSystem>();
                systemHandle.Update(World.Unmanaged);

                // Assert
                var visualTransform = MManager.GetComponentData<LocalTransform>(_visualEntity);
                Assert.AreEqual(scale, visualTransform.Scale, 0.001f, $"Visual should be scaled to {scale} when selected");

                // Reset for next iteration
                MManager.SetComponentData(_mainEntity, new Selected
                {
                    visualEntity = _visualEntity,
                    showScale = scale,
                    OnSelected = false,
                    OnDeselected = true
                });

                systemHandle.Update(World.Unmanaged);
            }
        }
    }
}