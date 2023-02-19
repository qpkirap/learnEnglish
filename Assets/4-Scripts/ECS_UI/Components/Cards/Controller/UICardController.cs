using Game.ECS.Components;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS_UI.Components
{
    public class UICardController : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private RectTransform container;

        [Header("First language")] 
        [SerializeField] private Text headText1;
        [SerializeField] private Text descText1; 
        [Header("Two language")] 
        [SerializeField] private Text headText2;
        [SerializeField] private Text descText2;

        [Header("Body")] 
        [SerializeField] private Button nexButton;

        private static EntityManager _manager => World.DefaultGameObjectInjectionWorld.EntityManager;
        private CompositeDisposable disposable = new();
        private Entity entity;
        private EntityManager manager;

        public RectTransform Root => root;
        
        public RectTransform Container => container;
        
        public Text HeadText1 => headText1;

        public Text DescText1 => descText1;

        public Text HeadText2 => headText2;

        public Text DescText2 => descText2;

        public Button NexButton => nexButton;

        public void Inject(Entity entity, EntityManager manager)
        {
            this.entity = entity;
            this.manager = manager;
        }

        private void OnClick()
        {
            if (entity != Entity.Null && !manager.HasComponent<CardMoveProcess>(entity))
            {
                manager.AddComponentData(entity, new ClickNextButtonTag());
                manager.AddComponentData(entity, new SpiralMoveTag());
            }
        }

        private void OnEnable()
        {
            nexButton.OnClickAsObservable().Subscribe(x =>
            {
                OnClick();
            }).AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }
    }
}