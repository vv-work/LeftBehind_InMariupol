using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _timerMax =0.2f;
        [SerializeField]
        private int _damage = 10;

        private class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var shootAttack = new ShootAttackData() {
                    TimerMax = authoring._timerMax,
                    Damage = authoring._damage,
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
    }
}