using System.Collections.Generic;
using Game.ECS.System;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Util;

namespace Game.ECS_UI.Components
{
    public class LeaderCanvas : MonoBehaviour
    {
        [SerializeField] private LeaderItem prefab;
        [SerializeField] private RectTransform rootItems;
        [SerializeField] private Button closeButton;

        private ObjectPool<LeaderItem> pool;

        private List<LeaderItem> spawnedItems = new();
        private readonly CompositeDisposable disp = new();

        public void InjectActivation(List<LeaderData> leaders)
        {
            pool ??= new ObjectPool<LeaderItem>(GetItem);
            InitSubscription();
            
            Disable();
            
            if (leaders == null) return;

            foreach (var leader in leaders)
            {
                if (leader == null) continue;
                
                var item = WarmItem();
                
                item.Inject(leader.nick, leader.pointClick.ToPrettyString());
            }
            
            gameObject.SetActive(true);
        }

        private LeaderItem WarmItem()
        {
            var item = pool.Get();
            spawnedItems.Add(item);
            item.gameObject.SetActive(true);
            
            return item;
        }
        
        private void ReleaseItem(LeaderItem item)
        {
            item.gameObject.SetActive(false);
            spawnedItems.Remove(item);
            
            pool.Release(item);
        }

        public void Disable()
        {
            foreach (var leaderItem in spawnedItems.ToArray())
            {
                ReleaseItem(leaderItem);
            }
            
            spawnedItems.Clear();
        }

        private void InitSubscription()
        {
            if (disp.Count > 0) return;
            
            closeButton.OnClickAsObservable().Subscribe(x =>
            {
                gameObject.SetActive(false);
                
                Disable();
            }).AddTo(disp);
        }
        
        private LeaderItem GetItem()
        {
            var instance = Instantiate(prefab, rootItems);
            instance.transform.localScale = Vector3.one;
            instance.gameObject.SetActive(true);

            return instance;
        }

        private void OnDestroy()
        {
            Disable();
            
            pool?.Dispose();
            disp?.Dispose();
        }
    }
}