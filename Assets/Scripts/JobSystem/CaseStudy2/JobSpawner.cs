using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace JobSystem.CaseStudy2
{
    public class JobSpawner : MonoBehaviour
    {
        private List<Transform> transforms = new List<Transform>();
        [SerializeField] Transform prefab;
        [SerializeField] int count;

        [SerializeField] private bool useJobs;
        [SerializeField] private bool jobInProgress;
        
        private NativeArray<float3> directions;
        private NativeArray<float> moveSpeeds;
        private NativeArray<float3> positions;
        private JobHandle jobHandle;
        [SerializeField] private int batchSize;
        
        [SerializeField] Button testButton;

        private void OnEnable()
        {
            testButton.onClick.AddListener(ChangeColor);
        }

        private void OnDisable()
        {
            testButton.onClick.RemoveListener(ChangeColor);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            
            directions = new NativeArray<float3>(count, Allocator.Persistent);
            moveSpeeds = new NativeArray<float>(count, Allocator.Persistent);
            positions = new NativeArray<float3>(count, Allocator.Persistent);

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new Vector3(UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10), UnityEngine.Random.Range(0, 10));
                transforms.Add(Instantiate(prefab, pos, Quaternion.identity));
                
                positions[i] = transforms[i].position;
                directions[i] = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
                moveSpeeds[i] = UnityEngine.Random.Range(-0.3f, 0.3f);
            }
        }

        private void Update()
        {
            HandleInput();
            
            if (useJobs)
            {
                MoveUsingJob();
            }
            else
            {
                jobHandle.Complete();
                for (int i = 0; i < count; i++)
                {
                    transforms[i].position += (Vector3)directions[i] * (moveSpeeds[i] * Time.deltaTime);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ChangeColor();
            }
        }

        private void ChangeColor()
        {
            for (int i = 0; i < count; i++)
            {
                transforms[i].gameObject.GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV();
            }
        }

        private void MoveUsingJob()
        {
            if (!jobInProgress)
            {
                MyParrellelJob job = new MyParrellelJob
                {
                    deltaTime = Time.deltaTime,
                    positions = positions,
                    directions = directions,
                    moveSpeeds = moveSpeeds
                };
                
                jobHandle = job.Schedule(count, batchSize);
                jobInProgress = true;
            }
            else if (jobHandle.IsCompleted)
            {
                jobHandle.Complete();
                
                // Update the positions of the transforms after the job is completed
                for (int i = 0; i < count; i++)
                {
                    transforms[i].position = positions[i];
                }
                
                jobInProgress = false;
            }
        }

        private void OnApplicationQuit()
        {
            jobHandle.Complete();
            if (directions.IsCreated)
            {
                directions.Dispose();
            }
            if (moveSpeeds.IsCreated)
            {
                moveSpeeds.Dispose();
            }
            if (positions.IsCreated)
            {
                positions.Dispose();
            }
            
            foreach (var transform in transforms)
            {
                Destroy(transform.gameObject);
            }
            
            transforms.Clear();
        }
    }
}