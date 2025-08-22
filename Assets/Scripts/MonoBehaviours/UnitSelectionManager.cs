using System;
using Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    public class UnitSelectionManager : UnityEngine.MonoBehaviour
    {

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                var mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                 EntityManager entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
                 EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMoverData>().Build(entityManager);
                 
                 NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
                 NativeArray<UnitMoverData> unitMoverArray = entityQuery.ToComponentDataArray<UnitMoverData>(Allocator.Temp);;
                 
                 for (int i = 0; i < unitMoverArray.Length; i++)
                 {
                     var unitMover = unitMoverArray[i];
                     unitMover.TargetPosition = mouseWorldPosition;
                     unitMoverArray[i] = unitMover;
                     //entityManager.SetComponentData(entityArray[i], unitMover); 
                 }
                 entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }
    }
}