using System;
using UnityEngine;

namespace JobSystem.CaseStudy5
{
    public class Calculation : MonoBehaviour
    {
        public struct Data
        {
            private int tmp;
            private int sum;
            private Unity.Mathematics.Random random;
            
            public Data(Calculation calculation)
            {
                tmp = calculation.floors * 10;
                sum = 0;
                random = new Unity.Mathematics.Random(1);
            }
            
            public void Update()
            {
                sum += tmp * random.NextInt(10, 100);
                Debug.Log(sum);
            }
        }

        private int floors = 10;
    }
}