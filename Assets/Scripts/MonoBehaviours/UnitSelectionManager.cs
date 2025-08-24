using System;
using Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public event EventHandler OnSelectionAreaStart; 
        public event EventHandler OnSelectionAreaEnd; 
        public static UnitSelectionManager Instance { get; private set; }
          
        private Vector2 _selectionStartPosition;
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _selectionStartPosition = Mouse.current.position.ReadValue();
                Rect selectionAreaRect = GetSelectionAreaRect();
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
                
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            { 
                var selectionEndPosition = Mouse.current.position.ReadValue();
                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }
                
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                var mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                 EntityManager entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
                 EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMoverData,Selected>().Build(entityManager);
                 
                 //NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
                 NativeArray<UnitMoverData> unitMoverArray = entityQuery.ToComponentDataArray<UnitMoverData>(Allocator.Temp);;
                 
                 for (int i = 0; i < unitMoverArray.Length; i++)
                 {
                     var unitMover = unitMoverArray[i];
                     unitMover.TargetPosition = mouseWorldPosition;
                     unitMoverArray[i] = unitMover;
                 }
                 entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }

        private Rect GetSelectionAreaRect()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var lowerLeftCorner = new Vector2(
                math.min(_selectionStartPosition.x,mousePos.x), 
                math.min(_selectionStartPosition.y, mousePos.y)
                );
            var upperRightCorner = new Vector2(
                math.max(_selectionStartPosition.x,mousePos.x), 
                math.max(_selectionStartPosition.y, mousePos.y)
            );
            return new Rect(lowerLeftCorner, upperRightCorner - lowerLeftCorner);
            
        }
    }
}