using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _timerMax =0.2f;
        [SerializeField]
        private int _damage = 10;

        [SerializeField]
        private float _attackDistance = 5;
        [SerializeField]
        private Transform _bulletSpawnerPoint;


        private class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var bulletOffset = Vector3.zero;
                    
                if(authoring._bulletSpawnerPoint!=null)
                     bulletOffset =authoring._bulletSpawnerPoint.localPosition;
                
                var shootAttack = new ShootAttackData() {
                    TimerMax = authoring._timerMax,
                    Damage = authoring._damage,
                    AttackDistance =  authoring._attackDistance,
                    BulletSpawnLocalPosition = bulletOffset
                };
                AddComponent(entity,shootAttack);
            }
        }
    }
    public struct ShootAttackData : IComponentData
    {
        public int Damage;
        public float TimerMax;
        public float Timer;
        public float AttackDistance;
        public float3 BulletSpawnLocalPosition;
    }
}