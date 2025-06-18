using System;
using UnityEngine;

namespace JobSystem.CaseStudy3
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float distThreshold;
        
        private Transform _target;

        private void Awake()
        {
            _target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            
            Quaternion lookRotation = Quaternion.AngleAxis(angle, transform.forward);
            transform.rotation = lookRotation;
            
            float distance = Vector3.Distance(transform.position, _target.position);
            if (distance < distThreshold)
            {
                transform.position += -direction * (speed * Time.deltaTime);
            }
        }
    }
}