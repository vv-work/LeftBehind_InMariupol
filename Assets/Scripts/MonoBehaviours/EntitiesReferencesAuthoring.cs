using UnityEngine;
using Unity.Entities;

namespace MonoBehaviours
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject bulletDataPrefab;
        [SerializeField] private GameObject zombiePrefab;
        
        private class EntitiesReferencesBaker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var entitiesReferencesData = new EntitiesReferencesData()
                {
                    BulletDataPrefabEntity = GetEntity(authoring.bulletDataPrefab, TransformUsageFlags.Dynamic),
                    ZombieEntity = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic)
                };
                AddComponent(entity, entitiesReferencesData);
            }
        }
    }
    
    public struct EntitiesReferencesData : IComponentData
    {
        public Entity BulletDataPrefabEntity;
        public Entity ZombieEntity;
    }
}