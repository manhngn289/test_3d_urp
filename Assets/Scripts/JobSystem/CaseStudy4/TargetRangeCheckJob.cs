using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace JobSystem.CaseStudy4
{
    [BurstCompile]
    public struct TargetRangeCheckJob : IJob
    {
        [ReadOnly] public NativeArray<float3> EnemyPositions;
        [ReadOnly] public NativeArray<bool> EnemyAttackableStates;
        [ReadOnly] public float3 PlayerPosition;
        [ReadOnly] public float DistanceCheck;
        
        [WriteOnly] public NativeReference<bool> HasEnemyInRange;
         
        
        public void Execute()
        {
            bool enemyFound = false;

            for (int i = 0; i < EnemyPositions.Length; i++)
            {
                if(!EnemyAttackableStates[i]) continue;

                float distance = math.distance(PlayerPosition, EnemyPositions[i]);

                if (distance <= DistanceCheck)
                {
                    enemyFound = true;
                    break;
                }
            }

            HasEnemyInRange.Value = enemyFound;
        }
    }
}