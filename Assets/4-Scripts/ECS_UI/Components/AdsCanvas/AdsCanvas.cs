using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS_UI.Components.AdsCanvas
{
    public class AdsCanvas : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button adsButton;

        private Entity _entity;
        private EntityManager EntityManager;

        private void Awake()
        {
            closeButton.OnClickAsObservable().Subscribe(x =>
            {
                Disable();
            }).AddTo(this);
            
            adsButton.OnClickAsObservable().Subscribe(x =>
            {
                ClickAdsButton();
            }).AddTo(this);
        }

        public void Disable()
        {
            gameObject.SetActive(false);

            EntityManager.AddComponent<CloseAdsCanvasTag>(_entity);
        }

        public void ClickAdsButton()
        {
            if (this._entity == Entity.Null) return;
            
            EntityManager.AddComponent<TryAdsViewTag>(this._entity);
        }

        public void Activate(Entity entity, EntityManager manager)
        {
            this._entity = entity;
            this.EntityManager = manager;

            gameObject.SetActive(true);
        }
    }

    public struct CloseAdsCanvasTag : IComponentData
    {
    }
    
    public struct TryAdsViewTag : IComponentData
    {
    }
}