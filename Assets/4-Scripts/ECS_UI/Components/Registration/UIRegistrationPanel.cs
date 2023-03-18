using Game.ECS.System;
using TMPro;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Game.ECS_UI.Components
{
    public class UIRegistrationPanel : UIItemControllerBase
    {
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passField;
        [SerializeField] private RegistrationButton nextButton;

        private string saveEmail;
        private string savePass;

        private static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        private void Awake()
        {
            emailField.ObserveEveryValueChanged(x => x.text).Subscribe(x =>
            {
                saveEmail = x;
            }).AddTo(this);
            
            passField.ObserveEveryValueChanged(x => x.text).Subscribe(x =>
            {
                savePass = x;
            }).AddTo(this);

            nextButton.OnClick.Subscribe(x=> TryRegistration()).AddTo(this);
        }

        private void TryRegistration()
        {
            if (entity != Entity.Null && EntityManager.HasComponent<AsyncTag>(entity)) return;
            
            if (!string.IsNullOrEmpty(saveEmail)
                && !string.IsNullOrEmpty(savePass))
            {
                entity = EntityManager.CreateEntity();

                var data = new FirebaseRegistrationData()
                {
                    email = saveEmail,
                    pass = savePass
                };

                EntityManager.AddComponentData(entity, data);
            }
        }

        public void SwitchStateButton(RegistrationButton.State state)
        {
            nextButton.SwitchState(state);
        }
    }

    public struct FirebaseRegistrationData : IComponentData
    {
        public FixedString512Bytes email;
        public FixedString512Bytes pass;
    }
}