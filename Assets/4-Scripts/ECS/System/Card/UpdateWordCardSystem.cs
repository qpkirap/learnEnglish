using Game.ECS_UI.Components;
using Game.ECS.Components;
using Game.ECS.System.SpawnCard;
using TextSpeech;
using Unity.Entities;

namespace Game.ECS.System
{
    [UpdateAfter(typeof(SpawnCardSystem))]
    public partial class UpdateWordCardSystem: UpdateSystem
    {
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        private static TextToSpeech textToSpeechSystem;
        private static LazyInject<GameState> gameState = new();

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
            InitTextToSpeech();
        }

        protected override void OnUpdate()
        {
            if (gameState.Value == null) return;
            
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();
            
            Entities
                .WithAll<CardTag, InstanceTag, DicElementData>()
                .WithNone<UpdateWordCardTag, CardMoveProcess>()
                .ForEach((Entity entity, in UICardControllerComponent uiCard, in DicElementData word) =>
            {
                uiCard.uiCardInstance.DescText1.text = word.Ru.Value;
                uiCard.uiCardInstance.DescText2.text = word.En.Value;

                ecb.AddComponent(entity, new UpdateWordCardTag());
                
                if (gameState.Value.SettingsState.IsActiveEngSpeech) textToSpeechSystem.StartSpeak(word.En.Value);

            }).WithStructuralChanges().WithoutBurst().Run();
        }

        private static void InitTextToSpeech()
        {
            TextToSpeech.Instance.Setting("en-US", 1, 1);
            textToSpeechSystem = TextToSpeech.Instance;
        }
    }
}