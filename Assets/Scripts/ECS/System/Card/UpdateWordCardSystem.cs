using Game.ECS_UI.Components;
using Game.ECS.Components;
using Unity.Entities;

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
                uiCard.uiCardInstance.DescText1.text = word.Ru.Value;
                uiCard.uiCardInstance.DescText2.text = word.En.Value;

                ecb.AddComponent(entity, new UpdateWordCardTag());

                //LayoutRebuilder.ForceRebuildLayoutImmediate(uiCard.uiCardInstance.Container);
                
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}