using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        private float _timerMax =0.2f;

        private class ShootAttackAuthoringBaker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var shootAttack = new ShootAttackData() {
                    TimerMax = authoring._timerMax,
                };
                AddComponent(entity,shootAttack);
            }
        }
    }
    public struct ShootAttackData : IComponentData
    {
        public float TimerMax;
        public float Timer;
    }
}