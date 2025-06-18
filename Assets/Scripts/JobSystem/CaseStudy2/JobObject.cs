using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace JobSystem.CaseStudy2
{
    public class JobObject : MonoBehaviour
    {
        [Header("Job Settings")]
        [SerializeField] private bool useJobs;

        private void Update()
        {
            if (useJobs)
            {
                JobSystemMultipleObjects();
            }
            else
            {
                print(Task(UnityEngine.Random.Range(-100000f, 100000f)));
            }
        }

        private float Task(float input)
        {
            float value = 0;
            for (int a = 0; a < 10; a++)
            {
                for (int i = 0; i < 10000; i++)
                {
                    value += math.sqrt(i);
                }
            }
            return input + value;
        }

        private void UseJobSystem()
        {
            // Schedule multiple jobs -> wait for all jobs to complete
            NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Temp);

            for (int i = 0; i < 10; i++)
            {
                MyJob myJob = new MyJob();
                JobHandle jobHandle = myJob.Schedule();
                handles.Add(jobHandle);
            }
            
            JobHandle.CompleteAll(handles.ToArray(Allocator.Temp));
            handles.Dispose();
        }

        private void JobSystemMultipleObjects()
        {
            NativeArray<float> output = new NativeArray<float>(1, Allocator.TempJob);
            MyJob myJob = new MyJob()
            {
                input = UnityEngine.Random.Range(-100000f, 100000f),
                output = output
            };
            JobHandle jobHandle = myJob.Schedule();
            jobHandle.Complete();
            
            print(output[0]);

            output.Dispose();
        }
    }
}