using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Authoring;

partial struct HealthBarSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
      foreach(var healthBar in SystemAPI.Query<RefRO<HealthBarData>>())
      {
        Vector3 cameraForward = Vector3.zero;
        if (UnityEngine.Camera.main != null)
          cameraForward = UnityEngine.Camera.main.transform.forward;

        HealthData health = SystemAPI.GetComponent<HealthData>(healthBar.ValueRO.HealthEntity);;
        
        float healthPercent = (float)health.Health / health.MaxHealth;

        //todo: write notes on PostTransformMatrix
        RefRW<PostTransformMatrix> ptm = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.BarVisualEntity);

        //todo: write notes on float4x4
        ptm.ValueRW.Value = float4x4.Scale(healthPercent, 1f, 1f);
        // RefRW<LocalTransform> lt  = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.BarVisualEntity) ;
        // lt.ValueRW.Scale = (healthPercent);


       }
    }
}
