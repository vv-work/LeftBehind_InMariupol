using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HealthAuthoring : MonoBehaviour
    {
        [SerializeField]
        private int _maxHealth =100;

        private class HealthAuthoringBaker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var health = new HealthData()
                {
                    MaxHealth = authoring._maxHealth,
                    Health = authoring._maxHealth
                };
                AddComponent(entity,health);
            }
        }
        
    }

    public struct HealthData : IComponentData
    {
        public int Health;
        public int MaxHealth;
    }
    
}