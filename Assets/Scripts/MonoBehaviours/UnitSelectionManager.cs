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
                EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager); 
                
                NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
                NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);;

                //Deselecting Units
                for (int i = 0; i < entityArray.Length; i++) {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                    Selected selected = selectedArray[i];
                    selected.OnDeselected = true;
                    selectedArray[i] = selected;
                    entityManager.SetComponentData(entityArray[i], selected);
                } 
                //entityQuery.CopyFromComponentDataArray(selectedArray);

                var selectionAreaRect = GetSelectionAreaRect();
                float selectionAreaSize = selectionAreaRect.height + selectionAreaRect.width;
                float multipleSelectionAreaSize = 50f;
                bool isMultipleSelect = selectionAreaSize > multipleSelectionAreaSize;
                    
                if (isMultipleSelect)
                    SelectUnitsInRectangle(entityManager, selectionAreaRect);
                else
                    PerformRaycastAndSelectUnit(entityManager, mousePosition);
                
                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }
                
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                var mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                 EntityManager entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
                 EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMoverData,Selected>().Build(entityManager);
                 
                 
                 NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
                 NativeArray<UnitMoverData> unitMoverArray = entityQuery.ToComponentDataArray<UnitMoverData>(Allocator.Temp);; 
                 
                 NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, unitMoverArray.Length);
                 
                 for (int i = 0; i < unitMoverArray.Length; i++)
                 {
                     var unitLocalTransform = unitMoverArray[i];
                     var unitMover = unitMoverArray[i];
                     unitMover.TargetPosition = movePositionArray[i];
                     unitMoverArray[i] = unitMover;
                 }
                 entityQuery.CopyFromComponentDataArray(unitMoverArray);
            }
        }

        private static void PerformRaycastAndSelectUnit(EntityManager entityManager, Vector2 mousePosition)
        {
            var entityQuery =  entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton)); 
            
            var physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorldSingleton.CollisionWorld; 
            var cameraRay = Camera.main.ScreenPointToRay(mousePosition);

            RaycastInput raycastInput = new RaycastInput()
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(1000f), 
                //todo: not Collision filter 
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = GameAssets.UNITY_LAYER,
                    GroupIndex = 0,
                }

            };

            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit)) {
                //todo: note .HasComponent<T>
                if (entityManager.HasComponent<UnitData>(hit.Entity)&& entityManager.HasComponent<Selected>(hit.Entity))
                { 
                    entityManager.SetComponentEnabled<Selected>(hit.Entity,true);
                    Selected selected = entityManager.GetComponentData<Selected>(hit.Entity);
                    selected.OnSelected = true;
                    entityManager.SetComponentData(hit.Entity,selected);
                } 
            }
        }

        private static void SelectUnitsInRectangle(EntityManager entityManager, Rect selectionAreaRect)
        {
            EntityQuery entityQuery;
            NativeArray<Entity> entityArray;
            //Selecting multiple logic
            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform,UnitData>().WithPresent<Selected>().Build(entityManager); 
            entityArray = entityQuery.ToEntityArray(Allocator.Temp);;
            NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);; 
                 
            // Selecting units in our rectangle
            for (int i = 0; i < localTransformArray.Length; i++)
            {
                var unitLocalTransform = localTransformArray[i];
                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                if (selectionAreaRect.Contains(unitScreenPosition))
                {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                    Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                    selected.OnSelected = true;
                    entityManager.SetComponentData(entityArray[i],selected);
                }
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

        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
        { 
            NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
            
            if (positionCount==0)
                return positionArray;
            
            positionArray[0] = targetPosition; 
            
            if (positionCount == 1)
                return positionArray;

            float ringSize = 1.2f;
            int ring = 0;
            int positionIndex = 0;

            while (positionIndex < positionCount)
            {
                int ringPositionCount = 3 + ring * 2;
                for(int i = 0; i < ringPositionCount; i++)
                {
                    float angle = i * (math.PI2/ringPositionCount);
                    float3 ringOffset = new float3(ringSize * (ring + 1),0,0);
                    //todo:note math.rotate() and `quaternion.RotateY` 
                    float3 ringVector = math.rotate(quaternion.RotateY(angle), ringOffset);
                    
                    float3 ringPosition = targetPosition + ringVector; 
                    positionArray[positionIndex] = ringPosition;
                    
                    positionIndex++;
                    if (positionIndex >= positionCount)
                        break;
                }

                ring++;
            }
            return positionArray;
        }
    }
}