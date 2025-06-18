using System;
using UnityEngine;

namespace JobSystem.CaseStudy3
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                transform.position += Vector3.forward * (moveSpeed * Time.deltaTime);
            }
            // float moveX = Input.GetAxis("Horizontal");
            // float moveZ = Input.GetAxis("Vertical");
            //
            // Vector3 move = new Vector3(moveX, 0, moveZ).normalized;
            // transform.position += move * (moveSpeed * Time.deltaTime);
        }
    }
}