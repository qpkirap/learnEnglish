using TMPro;
using UniRx;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


namespace Game.ECS_UI.Components
{
    public class UIRegistrationPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passField;
        [SerializeField] private Button nextButton;

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

            nextButton.OnClickAsObservable().Subscribe(x=>TryRegistration()).AddTo(this);
        }

        private void TryRegistration()
        {
            if (!string.IsNullOrEmpty(saveEmail)
                && !string.IsNullOrEmpty(savePass))
            {
                var entity = EntityManager.CreateEntity();

                var data = new FirebaseRegistrationData()
                {
                    email = saveEmail,
                    pass = savePass
                };

                EntityManager.AddComponentData(entity, data);
            }
        }
    }

    public struct FirebaseRegistrationData : IComponentData
    {
        public FixedString512Bytes email;
        public FixedString512Bytes pass;
    }
}