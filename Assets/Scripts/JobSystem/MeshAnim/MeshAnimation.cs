using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace JobSystem
{
    public class MeshAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 rippleOrigin;
        private Mesh mesh;
        private Vector3[] verticesArray, newVerticesArray;
        

        [Header("Job Objects")] 
        private MeshAnimJob job;
        private JobHandle jobHandle;
        private NativeArray<float3>[] vertexBuffers = new NativeArray<float3>[2];
        private NativeArray<float3> verticesNativeArray;
        private NativeArray<float3> newVerticesNativeArray;
        
        [Header("Job Settings")]
        private int bufferIndex = 0;
        [SerializeField] private bool usingLateUpdate;
        [SerializeField] private bool jobScheduled;

        private void Start()
        {
            // Get casual mesh data using C#
            mesh = GetComponent<MeshFilter>().mesh;
            
            // Get vertices data only for normal animation
            verticesArray = mesh.vertices;
            newVerticesArray = new Vector3[mesh.vertexCount];
            
            // Initialize NativeArrays for job
            verticesNativeArray = new NativeArray<float3>(mesh.vertexCount, Allocator.Persistent);
            newVerticesNativeArray = new NativeArray<float3>(mesh.vertexCount, Allocator.Persistent);
            
            // For double buffering, need 2 buffers
            for (int i = 0; i < 2; i++)
                vertexBuffers[i] = new NativeArray<float3>(mesh.vertexCount, Allocator.Persistent);
            
            // Copy vertices data to NativeArray buffers
            using (var meshData = Mesh.AcquireReadOnlyMeshData(mesh))
            {
                meshData[0].GetVertices(verticesNativeArray.Reinterpret<Vector3>());
                meshData[0].GetVertices(vertexBuffers[0].Reinterpret<Vector3>());
            }
            vertexBuffers[1].CopyFrom(vertexBuffers[0]);
            
            // Initialize job
            job = new MeshAnimJob
            {
                OriginalsVertices = verticesNativeArray,
                RipplePosition = Vector3.zero,
                
                // Write output to newVerticesNativeArray, NewVertices does not store results
                // Any changes are written to newVerticesNativeArray
                NewVertices = newVerticesNativeArray
            };
        }

        private void Update()
        {
            // Sorting by FPS
            JobifiedAsyncNextUpdate();
        }
        
        private void JobifiedAsyncNextUpdate()
        {
            if (jobHandle.IsCompleted && jobScheduled)
            {
                UpdateMesh();
                jobScheduled = false;
            }

            if (!jobScheduled)
            {
                job.deltaTime = Time.time;
            
                jobHandle = job.Schedule(verticesNativeArray.Length, 512);
                JobHandle.ScheduleBatchedJobs();
                jobScheduled = true;
            }
        }
        
        private void JobifiedAsyncDoubleBuffer()
        {
            int readIdx = bufferIndex;
            int writeIdx = 1 - bufferIndex;

            if (jobScheduled)
            {
                jobHandle.Complete();
                mesh.SetVertices(vertexBuffers[writeIdx].Reinterpret<Vector3>());
            }
            
            job.deltaTime = Time.time;
            job.NewVertices = vertexBuffers[writeIdx];

            jobHandle = job.Schedule(vertexBuffers[readIdx].Length, 512);
            JobHandle.ScheduleBatchedJobs();

            bufferIndex = writeIdx;
            jobScheduled = true;
        }

        private void JobifiedAnimate()
        {
            job.deltaTime = Time.time;
            
            jobHandle = job.Schedule(verticesNativeArray.Length, 512);
            
            UpdateMesh();
        }

        private void JobifiedAsyncLateUpdate()
        {
            job.deltaTime = Time.time;
            
            jobHandle = job.Schedule(verticesNativeArray.Length, 512);
            JobHandle.ScheduleBatchedJobs();
        }
        
        private void NormalAnimate()
        {
            for (int i = 0; i < verticesArray.Length; i++)
            {
                Vector3 originalVertex = verticesArray[i];
                float distance = Vector3.Distance(originalVertex, Vector3.zero);
                float rippleAmount = Mathf.Sin(distance - Time.time);
                Vector3 offset = (originalVertex - Vector3.zero).normalized * rippleAmount;
                Vector3 newPos = originalVertex + offset;

                newVerticesArray[i] = newPos;
            }
            mesh.SetVertices(newVerticesArray);
        }
        
        private void UpdateMesh()
        {
            jobHandle.Complete();
            mesh.SetVertices(newVerticesNativeArray.Reinterpret<Vector3>());
        }
        
        private void LateUpdate()
        {
            if(usingLateUpdate) UpdateMesh();
        }

        private void OnApplicationQuit()
        {
            jobHandle.Complete();
            
            if (verticesNativeArray.IsCreated) verticesNativeArray.Dispose();
            if (newVerticesNativeArray.IsCreated) newVerticesNativeArray.Dispose();
            
            for (int i = 0; i < 2; i++)
                if (vertexBuffers[i].IsCreated) vertexBuffers[i].Dispose();
        }
    }
}
