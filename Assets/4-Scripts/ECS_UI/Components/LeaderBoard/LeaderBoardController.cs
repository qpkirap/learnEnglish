using TMPro;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS_UI.Components
{
    public class LeaderBoardController : MonoBehaviour
    {
        public TMP_Text currentClickPoint;
        public TMP_Text leaderText;
        public TMP_Text leaderPoint;
        public TMP_Text currentNick;
        public Button button;
        
        private Entity entityCanvas;

        private EntityManager manager;


        private void Awake()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            button.OnClickAsObservable().Subscribe(x =>
            {
                ClickButton();
            }).AddTo(this);
        }

        private void ClickButton()
        {
            if (entityCanvas != Entity.Null)
            {
                manager.AddComponentData(entityCanvas, new LeaderBoardClickTag());
            }
        }

        public void Inject(Entity entityCanvas)
        {
            this.entityCanvas = entityCanvas;
        }
    }

    public struct LeaderBoardClickTag : IComponentData
    {
    }
}