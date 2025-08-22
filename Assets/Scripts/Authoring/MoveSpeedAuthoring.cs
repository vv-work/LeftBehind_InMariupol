using Unity.Entities;
using UnityEngine;

public class MoveSpeedAuthoring : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;
    private class MoveSpeedAuthoringBaker : Baker<MoveSpeedAuthoring>
    {
        public override void Bake(MoveSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            var moveSpeedData = new MoveSpeedData()
            {
                Value = authoring._moveSpeed
            }; 
            AddComponent(entity, moveSpeedData); 
        }
    }
}
public struct MoveSpeedData : IComponentData
{
    public float Value;
}
