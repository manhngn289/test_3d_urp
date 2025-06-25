using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS.Comparision
{
    public class ObjectSpawner : MonoBehaviour
    {
        private void Start()
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.CreateEntity();
            
        }
    }
}