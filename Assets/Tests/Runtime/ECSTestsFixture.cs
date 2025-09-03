using NUnit.Framework;
using Unity.Entities;
using Unity.Physics.Systems;
using UnityEngine;

namespace Tests
{
    /// <summary>
    /// Base class for ECS unit tests that provides common setup and teardown for World and EntityManager
    /// </summary>
    public abstract class ECSTestsFixture
    {
        protected World World;
        protected EntityManager m_Manager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Create a test world
            World = new World("TestWorld");
            m_Manager = World.EntityManager;

            OnCreate();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (World != null && World.IsCreated)
            {
                World.Dispose();
                World = null;
                m_Manager = default;
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
                m_Manager.DestroyEntity(m_Manager.UniversalQuery);
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