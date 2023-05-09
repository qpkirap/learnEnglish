using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Game.Ads
{
    public class AdsController : MonoBehaviour, IComponentData
    {
        [ReadOnly]
        public string idDemo = "demo-rewarded-yandex";
        [ReadOnly]
        public string idRewardAds = "R-M-2265338-2"; //видео с вознагрождением
        [ReadOnly]
        public string idInterstitionalAds = "R-M-2265338-3"; //полноэкранная реклама

        public Interstitial interstitialAd; //полноэкранная реклама
        public RewardedAd rewardedAd;
        public Entity entity;
        public EntityManager manager;
        
        //показывает только RewardedAd , Interstitial - почему то не работает 

        public void Awake()
        {
            MobileAds.SetUserConsent(true);

            PrepareInterstitialAds();
            PrepareRewardsAds();
        }

        private void PrepareInterstitialAds()
        {
            Debug.Log("PrepareInterstitialAds");
            
            interstitialAd = new Interstitial(idInterstitionalAds);
            AdRequest request = new AdRequest.Builder().Build();
            interstitialAd.LoadAd(request);
        }

        private void PrepareRewardsAds()
        {
            Debug.Log("PrepareRewardsAds");
            
            rewardedAd = new RewardedAd(idRewardAds);
            AdRequest request = new AdRequest.Builder().Build();
            rewardedAd.LoadAd(request);
        }

        public void InjectCanvas(Entity e, EntityManager manager)
        {
            this.entity = e;
            this.manager = manager;
        }

        public void ShowInterstitial()
        {
            if (interstitialAd.IsLoaded())
            {
                interstitialAd.OnReturnedToApplication += InterstitialOnOnReturnedToApplication;
                
                interstitialAd.OnLeftApplication += HandleLeftApplication;
                interstitialAd.OnAdClicked += HandleAdClicked;
         
                interstitialAd.OnImpression += HandleImpression;
                
                interstitialAd.Show();
            }
        }

        public void ShowRewarded()
        {
            Debug.Log($"ShowRewarded is load = {rewardedAd.IsLoaded()}");

            if (rewardedAd.IsLoaded())
            {
                rewardedAd.OnReturnedToApplication += RewardedOnOnReturnedToApplication;

                rewardedAd.OnLeftApplication += HandleLeftApplication;
                rewardedAd.OnAdClicked += HandleAdClicked;

                rewardedAd.OnImpression += HandleImpression;

                rewardedAd.Show();
            }
            else ErrorAds();
        }
        
        private void RewardedOnOnReturnedToApplication(object sender, EventArgs e)
        {
            Debug.Log($"InterstitialOnOnReturnedToApplication");
        }

        private void InterstitialOnOnReturnedToApplication(object sender, EventArgs e)
        {
            Debug.Log($"InterstitialOnOnReturnedToApplication");
            
            if (entity != Entity.Null)
            {
                manager.AddComponentData(entity, new AdsCompletedTag());
            }
            
            interstitialAd?.Destroy();
            
            PrepareInterstitialAds();
        }

        public void HandleLeftApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleLeftApplication event received");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdClicked event received");
        }

        public void HandleImpression(object sender, ImpressionData impressionData) //после нажатия на закрытие вознаграждения попадает сюда
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            MonoBehaviour.print("HandleImpression event received with data: " + data);
            
            CompleteRewardAds();
        }

        private void ErrorAds()
        {
            if (entity != Entity.Null)
            {
                manager.AddComponentData(entity, new AdsErrorTag());
            } 
        }

        private void CompleteRewardAds()
        {
            if (entity != Entity.Null)
            {
                manager.AddComponentData(entity, new AdsCompletedTag());
            }
            
            rewardedAd?.Destroy();
            
            PrepareInterstitialAds();
        }
        
        public void OnDestroy()
        {
            interstitialAd?.Destroy();
            rewardedAd?.Destroy();
        }
    }

    public struct AdsCompletedTag : IComponentData
    {
    }
    
    public struct AdsErrorTag : IComponentData
    {
    }
}