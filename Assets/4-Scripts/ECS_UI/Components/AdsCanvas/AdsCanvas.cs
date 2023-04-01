using System;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ECS_UI.Components.AdsCanvas
{
    public class AdsCanvas : MonoBehaviour, IDisposable
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button adsButton;

        private Entity _entity;

        private readonly CompositeDisposable disp = new();
        private EntityManager EntityManager;

        private void Awake()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            closeButton.OnClickAsObservable().Subscribe(x =>
            {
                gameObject.SetActive(false);

                EntityManager.AddComponent<CloseAdsCanvasTag>(_entity);
                
                disp.Clear();
                
            }).AddTo(this);
        }

        public void Activate(Entity entity)
        {
            this._entity = entity;
            
            adsButton.OnClickAsObservable().Subscribe(x =>
            {
                EntityManager.AddComponent<TryAdsViewTag>(entity);
            }).AddTo(disp);
            
            gameObject.SetActive(true);
        }

        public void Dispose()
        {
            disp?.Dispose();
        }
    }

    public struct CloseAdsCanvasTag : IComponentData
    {
    }
    
    public struct TryAdsViewTag : IComponentData
    {
    }
}