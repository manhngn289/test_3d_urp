using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace JobSystem.CaseStudy4
{
    [BurstCompile]
    public struct FindClosestTargetJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> UnitPositions;
        [ReadOnly] public NativeArray<float3> TargetPositions;
        // [ReadOnly] public NativeArray<float> UnitRanges;
        // [ReadOnly] public NativeArray<bool> TargetStates;
        
        [WriteOnly] public NativeArray<int> TargetIndices;
        
        
        public void Execute(int index)
        {
            // if (UnitRanges[index] <= 0)
            // {
            //     TargetIndices[index] = -1;
            //     return;
            // }
            
            TargetIndices[index] = -1;

            float3 unitPos = UnitPositions[index];
            // float attackRange = UnitRanges[index];

            int closestTargetIndex = -1;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < TargetPositions.Length; i++)
            {
                // if (!TargetStates[i]) continue;
                
                float distance = math.distance(unitPos, TargetPositions[i]);

                if (distance <= 10f && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTargetIndex = i;
                }
            }
            
            TargetIndices[index] = closestTargetIndex;
        }
    }
}