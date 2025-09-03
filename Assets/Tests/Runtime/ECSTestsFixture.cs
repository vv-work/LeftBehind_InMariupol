using NUnit.Framework;
using Unity.Entities;

namespace Tests.Runtime
{
    /// <summary>
    /// Base class for ECS unit tests that provides common setup and teardown for World and EntityManager
    /// </summary>
    public abstract class EcsTestsFixture
    {
        protected World World;
        protected EntityManager MManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Create a test world
            World = new World("TestWorld");
            MManager = World.EntityManager;

            OnCreate();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (World != null && World.IsCreated)
            {
                World.Dispose();
                World = null;
                MManager = default;
            }
        }

        [SetUp]
        public void SetUp()
        {
            OnSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up any entities created during the test
            if (World != null && World.IsCreated)
            {
                MManager.DestroyEntity(MManager.UniversalQuery);
            }
            OnTearDown();
        }

        /// <summary>
        /// Called once when the test fixture is created
        /// </summary>
        protected virtual void OnCreate() { }

        /// <summary>
        /// Called before each test
        /// </summary>
        protected virtual void OnSetUp() { }

        /// <summary>
        /// Called after each test
        /// </summary>
        protected virtual void OnTearDown() { }

        /// <summary>
        /// Update physics world to ensure collision detection works
        /// </summary>
        protected void UpdatePhysicsWorld()
        {
            // For testing, we'll simulate physics updates through the world update
            World.Update();
        }

        /// <summary>
        /// Simulate one frame by updating all systems
        /// </summary>
        protected void SimulateOneFrame()
        {
            World.Update();
        }
    }
}