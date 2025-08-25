using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SelectedAuthoring : MonoBehaviour
    {
        
        public GameObject VisualEntity;
        public float showScale = 1.5f;
        private class SelectedAuthoringBaker : Baker<SelectedAuthoring>
        {
            public override void Bake(SelectedAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Selected()
                {
                    visualEntity = GetEntity(authoring.VisualEntity, TransformUsageFlags.Dynamic),
                    showScale = authoring.showScale
                });
                SetComponentEnabled<Selected>(entity,false);
            }
        }
    }
}

public struct Selected : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float showScale;
    
    public bool OnSelected;
    public bool OnDeselected;


}