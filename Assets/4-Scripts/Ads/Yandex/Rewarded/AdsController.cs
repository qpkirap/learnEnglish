using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace Game.Ads
{
    public class AdsController : MonoBehaviour
    {
        private const string idDemo = "demo-rewarded-yandex";
        private const string id = "R-M-2265338-2";
        private RewardedAd rewardedAd;
        
        private void Awake()
        {
            rewardedAd = new RewardedAd(id);
            
            MobileAds.SetUserConsent(true);

            RequestRewardedAd();

            //Test();
        }

        private async UniTask Test()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            
            ShowRewardedAd();
        }
        
        private void RequestRewardedAd()
        {
            
            AdRequest request = new AdRequest.Builder().Build();

            AddSubscribeAds();

            rewardedAd.LoadAd(request);
        }
        
        private void ShowRewardedAd()
        {
            if (this.rewardedAd.IsLoaded())
            {
                rewardedAd.Show();
            }
            else
            {
                Debug.Log("Rewarded Ad is not ready yet");
            }
        }

        private void AddSubscribeAds()
        {
            rewardedAd.OnRewardedAdLoaded += HandleRewardedAdLoaded;
            rewardedAd.OnRewardedAdFailedToLoad += HandleRewardedAdFailedToLoad;
            rewardedAd.OnReturnedToApplication += HandleReturnedToApplication;
            rewardedAd.OnLeftApplication += HandleLeftApplication;
            rewardedAd.OnAdClicked += HandleAdClicked;
            rewardedAd.OnRewardedAdShown += HandleRewardedAdShown;
            rewardedAd.OnRewardedAdDismissed += HandleRewardedAdDismissed;
            rewardedAd.OnImpression += HandleImpression;
            rewardedAd.OnRewarded += HandleRewarded;
            rewardedAd.OnRewardedAdFailedToShow += HandleRewardedAdFailedToShow;
        }

        public void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdLoaded event received");
            
            ShowRewardedAd();
        }

        public void HandleRewardedAdFailedToLoad(object sender, AdFailureEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedAdFailedToLoad event received with message: " + args.Message);
        }

        public void HandleReturnedToApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleReturnedToApplication event received");
        }

        public void HandleLeftApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleLeftApplication event received");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdClicked event received");
        }

        public void HandleRewardedAdShown(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdShown event received");
        }

        public void HandleRewardedAdDismissed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdDismissed event received");
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            MonoBehaviour.print("HandleImpression event received with data: " + data);
        }

        public void HandleRewarded(object sender, Reward args)
        {
            MonoBehaviour.print("HandleRewarded event received: amout = " + args.amount + ", type = " + args.type);
        }

        public void HandleRewardedAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            MonoBehaviour.print(
                "HandleRewardedAdFailedToShow event received with message: " + args.Message);
        }

        private void OnDestroy()
        {
            rewardedAd.Destroy();
        }
    }
}