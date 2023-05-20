using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS_UI.Components
{
    public class SettingsCanvas : MonoBehaviour
    {
        [SerializeField] private RectTransform rootMove;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Button bgButton;
        [Space]
        [SerializeField] private GameObject scrollImage;

        [SerializeField] private Toggle autoNextToggle;
        [SerializeField] private Toggle engSpeechToggle;
        
        private Vector2 fingerDownPosition;
        private Vector2 fingerUpPosition;

        private Sequence sequenceRight;
        private Sequence sequenceLeft;
        private Vector2 sizeRootMove;
        private State currentState;

        private float minSwipeDistanceRight = 20f; // Минимальное расстояние свайпа для его считывания
        private float minSwipeDistanceLeft;
        
        private readonly LazyInject<GameState> gameState = new LazyInject<GameState>();

        private void Awake()
        {
            sizeRootMove = rootMove.sizeDelta;
            sequenceRight ??= DOTween.Sequence();
            sequenceLeft ??= DOTween.Sequence();

            minSwipeDistanceLeft = minSwipeDistanceRight * -1;

            autoNextToggle.OnValueChangedAsObservable().Subscribe(SwitchAutoNext).AddTo(this);
            bgButton.OnClickAsObservable().Subscribe(x=> OnClickBgButton()).AddTo(this);
            engSpeechToggle.OnValueChangedAsObservable().Subscribe(SwitchEngSpeech).AddTo(this);
            
            gameState.Value.SettingsState.SwitchEngSpeech(engSpeechToggle.isOn);
            gameState.Value.SettingsState.SwitchAutoNextCard(autoNextToggle.isOn);
        }

        private void Update()
        {
            // Проверяем, был ли касание начало свайпа
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                fingerDownPosition = Input.GetTouch(0).position;
            }

            // Проверяем, был ли отпускание пальца после свайпа
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                fingerUpPosition = Input.GetTouch(0).position;

                // Вычисляем величину и направление свайпа
                Vector2 swipeDelta = fingerUpPosition - fingerDownPosition;

                // Проверяем, является ли свайп вправо
                if (swipeDelta.x > minSwipeDistanceRight)
                {
                    if (!sequenceRight.IsPlaying())
                    {
                        sequenceLeft?.Kill();

                        sequenceRight = DOTween.Sequence().OnComplete(() => bgButton.gameObject.SetActive(true));
                        sequenceRight.Append(rootMove.DOAnchorPosX(0, 0.5f));
                        scrollImage.SetActive(false);

                        currentState = State.Active;
                    }
                }
                else if (minSwipeDistanceLeft > swipeDelta.x)
                {
                    if (!sequenceLeft.IsPlaying())
                    {
                        sequenceRight?.Kill();
                        
                        sequenceLeft = DOTween.Sequence().OnComplete(() => bgButton.gameObject.SetActive(false));;
                        sequenceLeft.Append(rootMove.DOAnchorPosX(-sizeRootMove.x, 0.5f));
                        scrollImage.SetActive(true);
                        
                        currentState = State.NotActive;
                    }
                    
                }
            }
        }

        private void OnClickBgButton()
        {
            switch (currentState)
            {
                case State.Active:
                {
                    sequenceLeft?.Kill();
                    
                    sequenceLeft = DOTween.Sequence().OnComplete(() => bgButton.gameObject.SetActive(false));;
                    sequenceLeft.Append(rootMove.DOAnchorPosX(-sizeRootMove.x, 0.5f));
                    scrollImage.SetActive(true);
                        
                    currentState = State.NotActive;
                }
                    break;
            }
        }

        private void SwitchAutoNext(bool isActive)
        {
            gameState.Value?.SettingsState.SwitchAutoNextCard(isActive);
        }
        
        private void SwitchEngSpeech(bool isActive)
        {
            gameState.Value?.SettingsState.SwitchEngSpeech(isActive);
        }
        
        private enum State
        {
            NotActive = 0,
            Active
        }
    }
}