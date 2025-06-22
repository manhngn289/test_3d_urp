using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace JobSystem.CaseStudy3
{
    [BurstCompile]
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int count;
        [SerializeField] private float spawnRadius;
        [SerializeField] private Transform playerTransform;
        
        [SerializeField] private float distThreshold;
        [SerializeField] private float speed;

        private bool isDone;

        private List<GameObject> enemies = new List<GameObject>();
        
        private TransformAccessArray transformAccessArray;
        private Transform[] enemyTransforms;

        private void Awake()
        {
            enemyTransforms = new Transform[count];
            
            for(int i = 0 ; i < count; i++)
            {
                Vector3 spawnPosition = UnityEngine.Random.insideUnitSphere * spawnRadius;
                spawnPosition.y = 0; // Ensure the enemy spawns on the ground
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                enemies.Add(enemy);
                enemyTransforms[i] = enemy.transform;
            }
            
            transformAccessArray = new TransformAccessArray(enemyTransforms);
            isDone = true;
        }

        private void Update()
        {
            if (isDone)
            {
                JobMoving();
            }
        }

        private void JobMoving()
        {
            ControlEnemyJob job = new ControlEnemyJob()
            {
                playerPos = playerTransform.position,
                deltaTime = Time.deltaTime,
                distThreshold = distThreshold,
                speed = speed
            };
            
            JobHandle handle = job.Schedule(transformAccessArray);
            handle.Complete();
        }

        private void NormalBehaviour()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                Vector3 direction = (playerTransform.position - enemies[i].transform.position).normalized;
                float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                enemies[i].transform.rotation = rotation;
                    
                float distance = Vector3.Distance(enemies[i].transform.position, playerTransform.position);
                if (distance <= distThreshold)
                {
                    enemies[i].transform.position += -direction * (speed * Time.deltaTime);
                }
                enemies[i].transform.position = new Vector3(enemies[i].transform.position.x, 0, enemies[i].transform.position.z);
            }
        }

        private void OnApplicationQuit()
        {
            transformAccessArray.Dispose();
        }
    }
}