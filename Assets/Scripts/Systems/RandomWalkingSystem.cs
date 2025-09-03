using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace Systems
{
    public partial struct RandomWalkingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach ((var randomWalking,var mover, var localTransform) 
                     in SystemAPI.Query<
                         RefRW<RandomWalkingData>, 
                         RefRW<UnitMoverData>, 
                         RefRO<LocalTransform> >()) 
            {

                if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.TargetPosition) <
                    UnitMoverSystem.REACHED_TARGET_POSITION_DISTANCE_SQ)
                {
                    //Reached target distance 
                    Random random = randomWalking.ValueRO.Random;

                    float3 randomDirection = math.normalize(new float3(random.NextFloat(-1,1),0,random.NextFloat(-1,1)));
                    float randomDistance = random.NextFloat(randomWalking.ValueRO.DistanceMin, randomWalking.ValueRO.DistanceMax);  
                    
                    randomWalking.ValueRW.TargetPosition = randomWalking.ValueRO.OriginPosition+
                                                           randomDirection * randomDistance; 
                    
                    randomWalking.ValueRW.Random = random; 
                    
                }
                else
                {
                    mover.ValueRW.TargetPosition = randomWalking.ValueRO.TargetPosition;
                }
                
                
            }

        }

    }
}