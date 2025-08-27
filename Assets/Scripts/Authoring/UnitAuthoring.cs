using MonoBehaviours;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class UnitAuthoring : MonoBehaviour
    {
        [SerializeField]
        private Faction _unitFaction;

        private class UnitAuthoringBaker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                var unit = new UnitData()
                {
                    Faction = authoring._unitFaction,
                };
                AddComponent(entity,unit); 
            }
        }
    }

    public struct UnitData : IComponentData
    {
        public Faction Faction;
    }
}