using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Game.Ads
{
    [GenerateAuthoringComponent]
    public class AdsController : IComponentData
    {
        [ReadOnly]
        public string idDemo = "demo-rewarded-yandex";
        [ReadOnly]
        public string id = "R-M-2265338-2";

        public Interstitial interstitial;
        
        private void Awake()
        {
            MobileAds.SetUserConsent(true);

            PrepareInterstitial();
        }

        private void PrepareInterstitial()
        {
            interstitial = new Interstitial(id);
            AdRequest request = new AdRequest.Builder().Build();
            interstitial.LoadAd(request);
        }

        public void ShowInterstitial()
        {
            if (interstitial.IsLoaded())
            {
                interstitial.OnReturnedToApplication += InterstitialOnOnReturnedToApplication;
                
                interstitial.OnLeftApplication += HandleLeftApplication;
                interstitial.OnAdClicked += HandleAdClicked;
         
                interstitial.OnImpression += HandleImpression;
                
                interstitial.Show();
            }
            
        }

        private void InterstitialOnOnReturnedToApplication(object sender, EventArgs e)
        {
           interstitial?.Destroy();
            
            PrepareInterstitial();
        }
        
        public void HandleLeftApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleLeftApplication event received");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdClicked event received");
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            MonoBehaviour.print("HandleImpression event received with data: " + data);
        }
        
        private void OnDestroy()
        {
            interstitial?.Destroy();
        }
    }
}