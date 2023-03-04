using Game.ECS_UI.Components;
using Game.ECS.Components;
using Game.ECS.System.SpawnCard;
using Unity.Entities;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(SpawnCardSystem))]
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
            
            Entities
                .WithAll<CardTag, InstanceTag, DicElementData>()
                .WithNone<UpdateWordCardTag, CardMoveProcess>()
                .ForEach((Entity entity, in UICardControllerComponent uiCard, in DicElementData word) =>
            {
                uiCard.uiCardInstance.DescText1.text = word.Ru.Value;
                uiCard.uiCardInstance.DescText2.text = word.En.Value;

                ecb.AddComponent(entity, new UpdateWordCardTag());

            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}