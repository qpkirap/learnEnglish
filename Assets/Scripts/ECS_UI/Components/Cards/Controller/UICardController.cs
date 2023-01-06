using System;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace CraftCar.ECS_UI.Components
{
    public class UICardController : MonoBehaviour
    {
        [SerializeField] private RectTransform root;

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

        public RectTransform Root => root;
        
        public Text HeadText1 => headText1;

        public Text DescText1 => descText1;

        public Text HeadText2 => headText2;

        public Text DescText2 => descText2;

        public Button NexButton => nexButton;

        private void OnEnable()
        {
            nexButton.OnClickAsObservable().Subscribe(x =>
            {
                
            }).AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }
    }
}