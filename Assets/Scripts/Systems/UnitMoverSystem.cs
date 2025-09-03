using Authoring;
using MonoBehaviours;
using Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial struct UnitMoverSystem : ISystem
    {

        public const float REACHED_TARGET_POSITION_DISTANCE_SQ = 2f;
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {

             UnitMoverJob job = new UnitMoverJob()
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency = job.ScheduleParallel(state.Dependency); 
        }

    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float DeltaTime;
    public void Execute(ref LocalTransform localTransform, in UnitMoverData unitMoverData,
        ref PhysicsVelocity physicsVelocity)
    {
         var targetPosition = unitMoverData.TargetPosition; //(float3)MouseWorldPosition.Instance.GetPosition();
        var moveDirection = targetPosition - localTransform.Position;


        if (math.lengthsq(moveDirection) <= UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return; 
        }
        
        
        moveDirection = math.normalize(moveDirection);

        float rotationSpeed = unitMoverData.RotationSpeed;
                
        var rot =  math.slerp(localTransform.Rotation, 
            quaternion.LookRotation(moveDirection, math.up()), 
            DeltaTime* rotationSpeed);
        localTransform.Rotation = rot;

        physicsVelocity.Angular = float3.zero;
        physicsVelocity.Linear = moveDirection * DeltaTime* unitMoverData.MovementSpeed;
        localTransform.Position = new float3(localTransform.Position.x,0.1f,localTransform.Position.z);
    }
}