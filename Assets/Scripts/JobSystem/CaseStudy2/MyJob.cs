using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace JobSystem.CaseStudy2
{
    [BurstCompile]
    public struct MyJob : IJob
    {
        // Input
        public float input;
        
        // Output
        public NativeArray<float> output;
        
        public void Execute()
        {
            float value = 0;
            for (int i = 0; i < 100000; i++)
            {
                value += math.sqrt(i);
            }
            output[0] = input + value;
        }
    }
}