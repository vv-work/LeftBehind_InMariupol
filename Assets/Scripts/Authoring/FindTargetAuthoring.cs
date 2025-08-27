using MonoBehaviours;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float _range;

        [SerializeField]
        private Faction _targetFaction;

        [SerializeField]
        private float _timerMax = 0.2f;

        private class FindTargetAuthoringBaker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
              Entity entity  = GetEntity(TransformUsageFlags.Dynamic);
              var fiendTarget = new FindTargetData
              {
                  Range = authoring._range,
                  TargetFaction = authoring._targetFaction,
                  TimerMax = authoring._timerMax,
                  
                  // TargetEntity = Entity.Null // Initialize with a default value
              };
              AddComponent(entity, fiendTarget);
            }
        }
    }

    public struct FindTargetData: IComponentData
    {
        public float Range; 
        public Faction TargetFaction;

        public float Timer;
        public float TimerMax;
        // public Entity TargetEntity;
    }
}
