using System;
using UnityEngine;

namespace ECS.Comparision
{
    public class Movement : MonoBehaviour
    {
        private float speed;

        private void Start()
        {
            speed = UnityEngine.Random.Range(1f, 10f);
        }

        private void Update()
        {
            Vector3 pos = transform.position;
            
            pos += transform.forward * (speed * Time.deltaTime);

            if (pos.z > 20)
            {
                pos.z = 0;
            }

            transform.position = pos;
        }
    }
}
