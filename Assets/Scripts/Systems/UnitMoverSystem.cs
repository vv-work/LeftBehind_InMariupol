using Authoring;
using MonoBehaviours;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    public partial struct UnitMoverSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {

            // foreach (var (localTransform, 
            //              unitMoverData,
            //              physicsVelocity) in
            //          SystemAPI.Query<
            //              RefRW<LocalTransform>, 
            //              RefRO<UnitMoverData>,
            //              RefRW<PhysicsVelocity>
            //          >()) { 
            //     
            //     var targetPosition = unitMoverData.ValueRO.TargetPosition; //(float3)MouseWorldPosition.Instance.GetPosition();
            //     var moveDirection = targetPosition - localTransform.ValueRW.Position; 
            //     moveDirection = math.normalize(moveDirection);
            //
            //     float rotationSpeed = unitMoverData.ValueRO.RotationSpeed;
            //     
            //     var rot =  math.slerp(localTransform.ValueRW.Rotation, 
            //         quaternion.LookRotation(moveDirection, math.up()), 
            //         SystemAPI.Time.DeltaTime * rotationSpeed);
            //     localTransform.ValueRW.Rotation = rot;
            //
            //     physicsVelocity.ValueRW.Angular = float3.zero;
            //     physicsVelocity.ValueRW.Linear = moveDirection * SystemAPI.Time.DeltaTime * unitMoverData.ValueRO.MovementSpeed;
            //
            // }
            UnitMoverJob job = new UnitMoverJob()
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);


        }

    }
}

public partial struct UnitMoverJob : IJobEntity
{
    public float DeltaTime;
    public void Execute(ref LocalTransform localTransform, in UnitMoverData unitMoverData,
        ref PhysicsVelocity physicsVelocity)
    {
         var targetPosition = unitMoverData.TargetPosition; //(float3)MouseWorldPosition.Instance.GetPosition();
        var moveDirection = targetPosition - localTransform.Position; 
        moveDirection = math.normalize(moveDirection);

        float rotationSpeed = unitMoverData.RotationSpeed;
                
        var rot =  math.slerp(localTransform.Rotation, 
            quaternion.LookRotation(moveDirection, math.up()), 
            DeltaTime* rotationSpeed);
        localTransform.Rotation = rot;

        physicsVelocity.Angular = float3.zero;
        physicsVelocity.Linear = moveDirection * DeltaTime* unitMoverData.MovementSpeed;
    }
}