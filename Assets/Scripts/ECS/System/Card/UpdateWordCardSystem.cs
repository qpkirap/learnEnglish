using CraftCar.ECS_UI.Components;
using CraftCar.ECS.Components;
using CraftCar.ECS.Components.SpawnData;
using Game.ECS.Components;
using Game.ECS.System.Base;
using Unity.Entities;
using UnityEngine.UI;

namespace Game.ECS.System
{
    public partial class UpdateWordCardSystem: UpdateSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();
            
            Entities.WithAll<CardTag, InstanceTag, DicElementData>().WithNone<UpdateWordCardTag>().ForEach((Entity entity, in UICardControllerComponent uiCard, in DicElementData word) =>
            {
                uiCard.uiCardInstance.DescText1.text = word.ru.Value;
                uiCard.uiCardInstance.DescText2.text = word.en.Value;

                ecb.AddComponent(entity, new UpdateWordCardTag());

                //LayoutRebuilder.ForceRebuildLayoutImmediate(uiCard.uiCardInstance.Container);
                
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}