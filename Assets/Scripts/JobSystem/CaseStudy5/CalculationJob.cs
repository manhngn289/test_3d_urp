using Unity.Collections;
using Unity.Jobs;

namespace JobSystem.CaseStudy5
{
    public struct CalculationJob : IJobParallelFor
    {
        public NativeArray<Calculation.Data> dataArray; 
        
        
        public void Execute(int index)
        {
            Calculation.Data data = dataArray[index];
            data.Update();
            dataArray[index] = data;
        }
    }
}