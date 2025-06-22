using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace JobSystem.CaseStudy4
{
    public class TargetSelector : MonoBehaviour
    {
        private FindClosestTargetJob findAllyTargetJob;
        private FindClosestTargetJob findEnemyTargetJob;
        private TargetRangeCheckJob activePlayerAttackJob;
        // private TargetRangeCheckJob activePlayerBattleCamJob;
        private EnemyAttackActiveJob activeEnemyAttackJob;
        
        // Ally data
        private NativeArray<float3> allyPositions;
        private NativeArray<float> allyAttackRanges;
        private NativeArray<int> targetOfAllies;
        private NativeArray<bool> allyActiveStates;
        
        // Enemy data
        private NativeArray<float3> enemyPositions;
        private NativeArray<float> enemyAttackRanges;
        private NativeArray<int> targetOfEnemies;
        private NativeArray<bool> enemyAttackableStates;
        private NativeArray<Quaternion> enemyRotations;
        private NativeArray<bool> useRectCheck;
        private NativeArray<float2> enemyRectSize;
        private NativeArray<float> enemyRadiusCheck;
        private NativeArray<bool> hasAllyInRange;
        
        // Player data
        private NativeReference<bool> activePlayerAttack;

        [SerializeField] private int previousAllyCount;
        [SerializeField] private int previousEnemyCount;

        public bool isStartSelectTarget;
        
        private JobHandle allyTargetJobHandle;
        private JobHandle enemyTargetJobHandle;
        private JobHandle playerAttackJobHandle;
        private JobHandle enemyAttackJobHandle;

        private List<GameObject> allies;
        private List<GameObject> enemies;

        private void Start()
        {
            int initialSize = 10;
        
            allyPositions = new NativeArray<float3>(initialSize, Allocator.Persistent);
            allyAttackRanges = new NativeArray<float>(initialSize, Allocator.Persistent);
            targetOfAllies = new NativeArray<int>(initialSize, Allocator.Persistent);
            allyActiveStates = new NativeArray<bool>(initialSize, Allocator.Persistent);

            enemyPositions = new NativeArray<float3>(initialSize, Allocator.Persistent);
            enemyAttackRanges = new NativeArray<float>(initialSize, Allocator.Persistent);
            targetOfEnemies = new NativeArray<int>(initialSize, Allocator.Persistent);
            enemyAttackableStates = new NativeArray<bool>(initialSize, Allocator.Persistent);
            enemyRotations = new NativeArray<Quaternion>(initialSize, Allocator.Persistent);
            useRectCheck = new NativeArray<bool>(initialSize, Allocator.Persistent);
            enemyRectSize = new NativeArray<float2>(initialSize, Allocator.Persistent);
            enemyRadiusCheck = new NativeArray<float>(initialSize, Allocator.Persistent);
            hasAllyInRange = new NativeArray<bool>(initialSize, Allocator.Persistent);

            activePlayerAttack = new NativeReference<bool>(Allocator.Persistent);
        }

        private void Update()
        {
            if (!isStartSelectTarget) return;
            
            CompleteAllJobs();
            
            CheckAndResizeArrays();
        }

        private void CheckAndResizeArrays()
        {
            int allyCount = allies.Count;
            int enemyCount = enemies.Count;
            
            if (allies.Count != previousAllyCount)
            {
                ReinitializeNativeArray(ref allyPositions, allyCount);
                ReinitializeNativeArray(ref allyAttackRanges, allyCount);
                ReinitializeNativeArray(ref targetOfAllies, allyCount);
                ReinitializeNativeArray(ref allyActiveStates, allyCount);
                previousAllyCount = allyCount;
            }
            
            if(enemyCount != previousEnemyCount)
            {
                ReinitializeNativeArray(ref enemyPositions, enemyCount);
                ReinitializeNativeArray(ref enemyAttackRanges, enemyCount);
                ReinitializeNativeArray(ref targetOfEnemies, enemyCount);
                ReinitializeNativeArray(ref enemyAttackableStates, enemyCount);
                ReinitializeNativeArray(ref enemyRotations, enemyCount);
                ReinitializeNativeArray(ref useRectCheck, enemyCount);
                ReinitializeNativeArray(ref enemyRectSize, enemyCount);
                ReinitializeNativeArray(ref enemyRadiusCheck, enemyCount);
                ReinitializeNativeArray(ref hasAllyInRange, enemyCount);
                previousEnemyCount = enemyCount;
            }
        }

        private void UpdateNativeArrayData()
        {
            for (int i = 0; i < allies.Count; ++i)
            {
                var ally = allies[i];
                allyPositions[i] = ally.transform.position;
                allyAttackRanges[i] = 100;
                allyActiveStates[i] = ally.gameObject.activeSelf;
            }
            
            
        }

        private void CompleteAllJobs()
        {
            allyTargetJobHandle.Complete();
            enemyTargetJobHandle.Complete();
            playerAttackJobHandle.Complete();
            enemyAttackJobHandle.Complete();
        }
        
        private void ReinitializeNativeArray<T>(ref NativeArray<T> array, int newSize) where T : struct
        {
            if (array.IsCreated)
            {
                array.Dispose();
            }
            array = new NativeArray<T>(newSize, Allocator.Persistent);
        }
    }
}