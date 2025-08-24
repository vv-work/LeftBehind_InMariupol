using System;
using Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public event EventHandler OnSelectionAreaStart; 
        public event EventHandler OnSelectionAreaEnd; 
        public static UnitSelectionManager Instance { get; private set; }

        [SerializeField] private LayerMask _unitLayerMask;
          
        private Vector2 _selectionStartPosition;
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _selectionStartPosition = mousePosition;
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
                
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            { 
                var selectionEndPosition = mousePosition;

                EntityManager entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithPresent<Selected>().Build(entityManager); 
                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);;

                //Deselecting Units
                for (int i = 0; i < entityArray.Length; i++) 
                    entityManager.SetComponentEnabled<Selected>(entityArray[i],false);
                
                
                var selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.height + selectionAreaRect.width;
                float multipleSelectionAreaSize = 50f;
                bool isMultipleSelect = selectionAreaSize > multipleSelectionAreaSize;
                    
                if (isMultipleSelect) {
                    
                    
                    //Selecting multiple logic
                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,Unit>().WithPresent<Selected>().Build(entityManager); 
                    entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
                    NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);; 
                 
                    // Selecting units in our rectangle
                    for (int i = 0; i < localTransformArray.Length; i++)
                    {
                        var unitLocalTransform = localTransformArray[i];
                        Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                        if (selectionAreaRect.Contains(unitScreenPosition))
                            entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                    }
                }
                else
                { 
                    //todo: Write notes about Physics
                    entityQuery =  entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    //todo: Note taking singleton 
                    var physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    var collisionWorld = physicsWorldSingleton.CollisionWorld; 
                    var cameraRay = Camera.main.ScreenPointToRay(mousePosition);

                    int unitLayer = 7;
                    RaycastInput raycastInput = new RaycastInput()
                    {
                        //todo:note Geting point
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(1000f), 
                        //todo: not Collision filter
                        Filter = new CollisionFilter()
                        {
                            GroupIndex = 0,
                            BelongsTo = ~0u,
                            // CollidesWith = (uint)_unitLayerMask.value, 
                            CollidesWith = 1u<<unitLayer
                        }

                    };

                    if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit)) {
                        //todo: note .HasComponent<T>
                        if (entityManager.HasComponent<Unit>(hit.Entity))
                        { 
                           entityManager.SetComponentEnabled<Selected>(hit.Entity,true);
                        } 
                    }

  

                }
                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }
                
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                var mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                 EntityManager entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
                 EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMoverData,Selected>().Build(entityManager);
                 
                 
                 NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
                 NativeArray<UnitMoverData> unitMoverArray = entityQuery.ToComponentDataArray<UnitMoverData>(Allocator.Temp);; 
                 
                 for (int i = 0; i < unitMoverArray.Length; i++)
                 {
                     var unitLocalTransform = unitMoverArray[i];
                     var unitMover = unitMoverArray[i];
                     unitMover.TargetPosition = mouseWorldPosition;
                     unitMoverArray[i] = unitMover;
                 }
                 entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }

        public Rect GetSelectionAreaRect()
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

        private NativeArray<float3> GenerateMovePositionArray(float targetPosition, int positionCount)
        {

            NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
            
            if (positionCount==0)
                return positionArray;
            
            positionArray[0] = targetPosition; 
            if (positionCount == 1)
                return positionArray; 
            
            return positionArray;
        }
    }
}