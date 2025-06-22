using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace JobSystem
{
    [BurstCompile]
    public struct MeshAnimJob : IJobParallelFor
    {
        // Input
        [ReadOnly] public NativeArray<float3> OriginalsVertices;
        [ReadOnly] public float3 RipplePosition;
        [ReadOnly] public float deltaTime;

        // Output
        [WriteOnly] public NativeArray<float3> NewVertices;
        
        
        
        public void Execute(int index)
        {
            float3 originalVertex = OriginalsVertices[index];
            float distance = math.distance(originalVertex, RipplePosition);
            float rippleAmount = math.sin(distance - deltaTime);
            float3 offset = math.normalize(originalVertex - RipplePosition) * rippleAmount;
            float3 newPos = originalVertex + offset;

            NewVertices[index] = newPos;
        }
    }
}