using Unity.Entities;
using UnityEngine;

namespace MonoBehaviours
{
    public class BulletAuthoring : MonoBehaviour
   {
 
       [SerializeField]
        private float _speed = 2f;

       [SerializeField] private int _damageAmount = 5;


        private class BulletAuthoringBaker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var bullet = new BulletData()
                {
                        Speed = authoring._speed,
                        DamageAmount = authoring._damageAmount,

                };
                AddComponent(entity,bullet);
                
            }
        }
    }

    public struct BulletData : IComponentData
    {
        public float Speed;
        public int DamageAmount; 
    }
}