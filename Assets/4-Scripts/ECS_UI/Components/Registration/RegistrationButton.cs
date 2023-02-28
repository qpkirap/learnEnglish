using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS_UI.Components
{
    public class RegistrationButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [Header("States")] 
        [SerializeField] private GameObject createUserState;
        [SerializeField] private GameObject processState;

        public IObservable<Unit> OnClick => button.OnClickAsObservable();

        public void SwitchState(State state)
        {
            createUserState.SetActive(state == State.CreateUser);
            processState.SetActive(state == State.Process);
        }
        
        public enum State
        {
            CreateUser,
            Process
        }
    }
}