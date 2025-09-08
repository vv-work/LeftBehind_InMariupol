using UnityEngine;
using Unity.Entities;


public class HealthBarAuthoring : MonoBehaviour
{
  public GameObject BarVisualGameObject;
  public GameObject HealthGameObject;

  class Bakser : Baker<HealthBarAuthoring>{
    public override void Bake(HealthBarAuthoring authoring)
    {
      var entity = GetEntity(TransformUsageFlags.Dynamic);
      var data = new HealthBarData(){
        //todo: Make notes about TransformUsageFlags.NonUniformScale
        
        BarVisualEntity = GetEntity(authoring.BarVisualGameObject, TransformUsageFlags.NonUniformScale),
        HealthEntity = GetEntity(authoring.HealthGameObject, TransformUsageFlags.Dynamic),

      };

      AddComponent(entity, data);

    }
  }

}

public struct HealthBarData: IComponentData{
  public Entity BarVisualEntity;
  public Entity HealthEntity;

}




    
