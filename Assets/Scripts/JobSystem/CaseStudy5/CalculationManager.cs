using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JobSystem.CaseStudy5
{
    public class CalculationManager : MonoBehaviour
    {
        [SerializeField] private List<Calculation> calculations;

        [SerializeField] private Calculation prefabCalculation;
        
        NativeArray<Calculation.Data> calculationDataArray;

        private CalculationJob job;

        private JobHandle jobHandle;
        
        private void Start()
        {
            calculations = new List<Calculation>();
            for (int i = 0; i < 50; i++)
            {
                calculations.Add(Instantiate(prefabCalculation, new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)), Quaternion.identity));
            }
            
            calculationDataArray = new NativeArray<Calculation.Data>(calculations.Count, Allocator.Persistent);
            for (int i = 0; i < calculations.Count; i++)
            {
                calculationDataArray[i] = new Calculation.Data(calculations[i]);
            }
            
            job = new CalculationJob()
            {
                dataArray = calculationDataArray
            };
        }

        private void Update()
        {
            job = new CalculationJob()
            {
                dataArray = calculationDataArray
            };
            jobHandle = job.Schedule(calculations.Count, 64);
            jobHandle.Complete();
        }

        private void OnApplicationQuit()
        {
            if (calculationDataArray.IsCreated)
            {
                calculationDataArray.Dispose();
            }
            
            foreach (var calculation in calculations)
            {
                Destroy(calculation.gameObject);
            }
            calculations.Clear();
        }
    }
}