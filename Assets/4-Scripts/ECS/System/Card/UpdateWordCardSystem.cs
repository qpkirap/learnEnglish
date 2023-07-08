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
        private LazyInject<GameState> gameState = new();
        
        private static SpeechType lastSpeechType = SpeechType.None;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
            textToSpeechSystem ??= TextToSpeech.Instance;

            textToSpeechSystem.onDoneCallback += () =>
            {
                Entities
                    .WithAll<SpeechPlayed>()
                    .ForEach((Entity e, in SpeechPlayed played) =>
                {
                    var c = new SpeechComplete(played.play);
                    
                    EntityManager.AddComponentData(e, c);

                    if (played.play == SpeechType.Rus)
                    {
                        EntityManager.AddComponentData(e, new SpeechAllCompleteTag());
                    }
                }).WithStructuralChanges().Run();
            };
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

                if (gameState.Value.SettingsState.IsActiveEngSpeech)
                {
                    ecb.AddComponent(entity, new SpeechPlayed(SpeechType.Eng));
                    textToSpeechSystem.StartSpeak(word.En.Value);
                }

            }).WithStructuralChanges().WithoutBurst().Run();
            
            //Воспроизводим английский
            Entities
                .WithAll<UpdateWordCardTag, InstanceTag, DicElementData>()
                .WithNone<SpeechPlayed>()
                .ForEach((Entity entity, in UICardControllerComponent uiCard, in DicElementData word) =>
                {
                    if (gameState.Value.SettingsState.IsActiveEngSpeech)
                    {
                        InitEngTextToSpeech();
                        
                        ecb.AddComponent(entity, new SpeechPlayed(SpeechType.Eng));
                        textToSpeechSystem.StartSpeak(word.En.Value);
                    }
                    else
                    {
                        ecb.AddComponent(entity, new SpeechPlayed(SpeechType.Eng));
                        ecb.AddComponent(entity, new SpeechComplete(SpeechType.Eng));
                    }
                }).WithStructuralChanges().WithoutBurst().Run();
            
            //руссиано
            Entities
                .WithAll<SpeechPlayed, SpeechComplete, DicElementData>()
                .ForEach((Entity entity, in SpeechPlayed played, in DicElementData word) =>
                {
                    ecb.RemoveComponent<SpeechComplete>(entity);
                    
                    if (played.play == SpeechType.Rus) return;
                    
                    if (gameState.Value.SettingsState.IsActiveRusSpeech)
                    {
                        InitRusTextToSpeech();
                        
                        ecb.AddComponent(entity, new SpeechPlayed(SpeechType.Rus));
                        textToSpeechSystem.StartSpeak(word.Ru.Value);
                    }
                    else
                    {
                        ecb.AddComponent(entity, new SpeechPlayed(SpeechType.Rus));
                        ecb.AddComponent(entity, new SpeechComplete(SpeechType.Rus));
                        ecb.AddComponent(entity, new SpeechAllCompleteTag());
                    }
                }).WithStructuralChanges().WithoutBurst().Run();
        }
        

        private static void InitEngTextToSpeech()
        {
            TextToSpeech.Instance.Setting("en-US", 1, 1);
        }
        
        private static void InitRusTextToSpeech()
        {
            TextToSpeech.Instance.Setting("ru-Ru", 1, 1);
        }
    }

    public struct SpeechComplete : IComponentData
    {
        public SpeechType complete;

        public SpeechComplete(SpeechType complete)
        {
            this.complete = complete;
        }
    }
    
    public struct SpeechPlayed : IComponentData
    {
        public SpeechPlayed(SpeechType type)
        {
            play = type;
        }
        
        public SpeechType play;
    }
    
    public struct SpeechAllCompleteTag : IComponentData
    {
    }

    public enum SpeechType
    {
        None,
        Eng,
        Rus
    }
}