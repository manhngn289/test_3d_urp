using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace JobSystem.CaseStudy3
{
    [BurstCompile]
    public struct ControlEnemyJob : IJobParallelForTransform
    {
        [ReadOnly] public float3 playerPos;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float distThreshold;
        [ReadOnly] public float speed;
        
        public void Execute(int index, TransformAccess transform)
        {
            float3 direction = math.normalize(playerPos - (float3)transform.position);
            float angle = math.atan2(direction.z, direction.x);

            quaternion rotation = quaternion.AxisAngle(new float3(0, 0, 1), angle);
            transform.rotation = rotation;
            
            float distance = math.distance(transform.position, playerPos);
            float3 newPos = (float3)transform.position - direction * deltaTime * speed;
            float3 maskedPos = math.select(transform.position, newPos, distance <= distThreshold);
            transform.position = maskedPos;
        }
    }
}