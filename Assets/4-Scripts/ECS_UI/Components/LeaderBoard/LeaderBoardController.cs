using System;
using System.Collections.Generic;
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
        public Button tryRegistrationButton;
        
        public List<LeaderBoardParam> viewParams;

        private Entity entityCanvas;

        private EntityManager manager;


        private void OnEnable()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            button.OnClickAsObservable().Subscribe(x =>
            {
                ClickButton();
            }).AddTo(this);
            
            tryRegistrationButton.OnClickAsObservable().Subscribe(x =>
            {
                TryRegistration();
            }).AddTo(this);
        }

        private void ClickButton()
        {
            if (entityCanvas != Entity.Null)
            {
                manager.AddComponentData(entityCanvas, new LeaderBoardClickTag());
            }
        }
        
        private void TryRegistration()
        {
            if (entityCanvas != Entity.Null)
            {
                manager.AddComponentData(entityCanvas, new LeaderBoardTryRegistrationTag());
            }
        }

        public void Inject(Entity entityCanvas)
        {
            this.entityCanvas = entityCanvas;
        }

        public void SwitchState(State state)
        {
            if (viewParams == null) return;

            foreach (var leaderBoardParam in viewParams)
            {
                if (leaderBoardParam == null) continue;
                if (leaderBoardParam.root == null) continue;
                
                leaderBoardParam.root.SetActive(state == leaderBoardParam.state);
            }
            
            Debug.Log(state);
        }
        
        public enum State
        {
            RegComplete,
            NotRegistration
        }
        
        [Serializable]
        public class LeaderBoardParam
        {
            [field: SerializeField] public State state;
            [field: SerializeField] public GameObject root;
        }
    }
    
    public struct LeaderBoardTryRegistrationTag : IComponentData
    {
    }

    public struct LeaderBoardClickTag : IComponentData
    {
    }
}