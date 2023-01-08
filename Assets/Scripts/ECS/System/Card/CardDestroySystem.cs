using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components.Tags;
using Game.CustomPool;
using Unity.Entities;

namespace CraftCar.ECS.System.Card
{
    public partial class CardDestroySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<UICardControllerComponent, DestroyTag>()
                .ForEach((Entity e, UICardControllerComponent card) =>
                {
                    PoolManager.Instance.ReleaseObject(card.Instance.gameObject);
                    
                    EntityManager.DestroyEntity(e);
                }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}