using CraftCar.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace CraftCar.ECS.System.SpawnCard
{
    [AlwaysUpdateSystem]
    public partial class SpawnCardSystem : SystemBase
    {

        protected override void OnCreate()
        {
            Debug.Log("SpawnCardSystem create");
        }
        
        protected override void OnUpdate()
        {
            int countCard = 0;

            Entities.WithAll<CardTag>().ForEach((Entity e) =>
            {
                countCard++;
            }).WithoutBurst().Run();

            if (countCard == 0)
            {
                var newEntity = EntityManager.CreateEntity(typeof(CardTag));

                EntityManager.AddComponent<CardTag>(newEntity);
            }
            
        }
    }
}