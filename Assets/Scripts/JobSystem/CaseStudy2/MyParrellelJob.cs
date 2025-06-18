using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace JobSystem.CaseStudy2
{
    [BurstCompile]
    public struct MyParrellelJob : IJobParallelFor
    {
        public float deltaTime;
        public NativeArray<float3> positions;
        public NativeArray<float3> directions;
        public NativeArray<float> moveSpeeds;
        
        public void Execute(int index)
        {
            positions[index] += directions[index] * moveSpeeds[index] * deltaTime;
        }
    }
}