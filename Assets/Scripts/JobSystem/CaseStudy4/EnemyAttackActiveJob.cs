using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace JobSystem.CaseStudy4
{
    [BurstCompile]
    public struct EnemyAttackActiveJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> EnemyPositions;
        [ReadOnly] public NativeArray<float3> AllyPositions;
        [ReadOnly] public NativeArray<quaternion> EnemyRotations;
        [ReadOnly] public NativeArray<bool> UseRect;
        [ReadOnly] public NativeArray<float2> EnemyRectCheck;
        [ReadOnly] public NativeArray<float> EnemyRadiusCheck;
        [ReadOnly] public NativeArray<bool> AllyStates;

        [WriteOnly] public NativeArray<bool> HasAllyInRange;
        
        
        public void Execute(int index)
        {
            float3 enemyPos = EnemyPositions[index];
            quaternion enemyRot = EnemyRotations[index];
            bool hasAlly = true;

            for (int i = 0; i < AllyPositions.Length; i++)
            {
                if (!AllyStates[i]) continue;

                float3 allyPos = AllyPositions[i];
                float3 directionToAlly = allyPos - enemyPos;

                if (UseRect[index])
                {
                    // World -> Local space
                    float3 localDirection = math.mul(math.inverse(enemyRot), directionToAlly);

                    float2 rectSize = EnemyRectCheck[index];

                    if (math.abs(localDirection.x) <= rectSize.x * 0.5f &&
                        math.abs(localDirection.z) <= rectSize.y * 0.5f &&
                        localDirection.z >= 0)
                    {
                        hasAlly = true;
                        break;
                    }
                }
                else
                {
                    float distance = math.length(directionToAlly);

                    if (distance <= EnemyRadiusCheck[index])
                    {
                        hasAlly = true;
                        break;
                    }
                }
            }
            
            HasAllyInRange[index] = hasAlly;
        }
    }
}