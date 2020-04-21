using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private bool _testMode;
    private bool _enablePerPlacementLoad;
    private RewardBasedVideoAd _adMobRewardedVideoAd;
    private InterstitialAd _adMobInterstitialAd;
    private BannerView _adMobBannerView;
    private string _adMobGameId;
    private string _unityGameId;
    private string _adMobRewardedVideoAdId;
    private string _adMobInterstitialAdId;
    private string _adMobMenuBannerAdId;
    private string _unityRewardedVideoPlacementId;
    private string _unityInterstitialPlacementId;
    private string _unityMenuBannerPlacementId;

    public void SetTestMode(bool testMode)
    {
        _testMode = testMode;
    }

    public void SetEnablePerPlacementLoad(bool enablePerPlacementLoad)
    {
        _enablePerPlacementLoad = enablePerPlacementLoad;
    }
    
    public void SetAdMobGameId(string adMobGameId)
    {
        _adMobGameId = adMobGameId;
    }

    public void SetUnityGameId(string unityGameId)
    {
        _unityGameId = unityGameId;
    }

    public void SetAdMobRewardedVideoId(string adMobRewardedVideoId)
    {
        _adMobRewardedVideoAdId = adMobRewardedVideoId;
    }


    public void SetAdMobInterstitialAdId(string adMobInterstitialVideoId)
    {
        _adMobInterstitialAdId = adMobInterstitialVideoId;
    }

    public void SetAdMobMenuBannerAdId(string adMobMenuBannerAdId)
    {
        _adMobMenuBannerAdId = adMobMenuBannerAdId;
    }

    public void SetUnityRewardedVideoPlacementId(string unityRewardedVideoPlacementId)
    {
        _unityRewardedVideoPlacementId = unityRewardedVideoPlacementId;
    }

    public void SetUnityInterstitialPlacementId(string unityInterstitialPlacementId)
    {
        _unityInterstitialPlacementId = unityInterstitialPlacementId;
    }

    public void SetUnityMenuBannerPlacementId(string unityMenuBannerPlacementId)
    {
        _unityMenuBannerPlacementId = unityMenuBannerPlacementId;
    }

    public void OnUnityAdsReady (string placementId) {
        GameDriver.SendMessage("Unity ad is ready", false);
    }
    
    public void OnUnityAdsDidError (string errorMessage) {
        GameDriver.SendMessage(errorMessage, true);
    }
    
    public void OnUnityAdsDidStart (string placementId) {
        GameDriver.SendMessage("The ad started playing", false);
    }
    
    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        if(showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
            GameDriver.EarnACoin();
            GameDriver.SendMessage("Player earned a coin", false);
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
            GameDriver.LoseACoin();
            GameDriver.SendMessage("Player lost a coin", false);
        } else if (showResult == ShowResult.Failed) {
            GameDriver.SendMessage("The ad did not finish due to an error", true);
        }
    }

    private void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleRewardBasedVideoLoaded event received", false);
        _adMobRewardedVideoAd.Show();
    }

    private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        GameDriver.SendMessage("HandleRewardBasedVideoFailedToLoad event received with message: "+args.Message, true);
    }

    private void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleRewardBasedVideoOpened event received", false);
    }

    private void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleRewardBasedVideoStarted event received", false);
    }
    
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        GameDriver.SendMessage("HandleRewardBasedVideoRewarded event received for "+amount+" "+type, false);
        GameDriver.EarnAToken();
    }

    private void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleRewardBasedVideoClosed event received", false);
    }

    private void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleRewardBasedVideoLeftApplication event received", false);
    }

    private void HandleInterstitialAdLoaded(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleAdLoaded event received", false);
        _adMobInterstitialAd.Show();
    }

    private void HandleInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        GameDriver.SendMessage("HandleFailedToReceiveAd event received with message: "+args.Message, true);
    }

    private void HandleInterstitialAdOpened(object sender, EventArgs args)
    {
        //print("HandleAdOpened event received");
        GameDriver.SendMessage("HandleAdOpened event received", false);
    }

    private void HandleInterstitialAdClosed(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleAdClosed event received", false);
        //_adMobInterstitialAd.Destroy();
    }

    private void HandleInterstitialAdLeavingApplication(object sender, EventArgs args)
    {
        GameDriver.SendMessage("HandleAdLeavingApplication event received", false);
        //_adMobInterstitialAd.Destroy();
    }

    public void AdMobEnableRewardedVideoAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        _adMobRewardedVideoAd.LoadAd(request, _adMobRewardedVideoAdId);
    }

    public void AdMobEnableInterstitialAd()
    {
        _adMobInterstitialAd = new InterstitialAd(_adMobInterstitialAdId);
        AdRequest request = new AdRequest.Builder().Build();
        _adMobInterstitialAd.LoadAd(request);
    }

    public void AdMobEnableBannerAd()
    {
        _adMobBannerView = new BannerView(_adMobMenuBannerAdId, AdSize.Banner, AdPosition.Top);
        AdRequest request = new AdRequest.Builder().Build();
        _adMobBannerView.LoadAd(request);
        GameDriver.SendMessage("Start AdMob timer", false);
    }

    public void UnityEnableRewardedVideoAd()
    {
        Advertisement.Load(_unityRewardedVideoPlacementId);
        Advertisement.Show(_unityRewardedVideoPlacementId);
    }
    
    public void UnityEnableInterstitialAd()
    {
        Advertisement.Load(_unityInterstitialPlacementId);
        Advertisement.Show(_unityInterstitialPlacementId);
    }
    
    public void UnityEnableBannerAd()
    {
        Advertisement.Banner.Load();
        Advertisement.Banner.Show(_unityMenuBannerPlacementId);
        GameDriver.SendMessage("Start Unity timer", false);
    }

    public void DestroyBanners(bool isUnityBanner)
    {
        if (isUnityBanner)
        {
            Advertisement.Banner.Hide();
        }
        else
        {
            _adMobBannerView.Destroy();
        }
    }

    // Start is called before the first frame update
    public void SetUpAdvertising()
    {
        Advertisement.AddListener(GetComponent<IUnityAdsListener>());
        Advertisement.Initialize(_unityGameId, _testMode, _enablePerPlacementLoad);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        _adMobRewardedVideoAd = RewardBasedVideoAd.Instance;
        AddListeners();
        MobileAds.Initialize(_adMobGameId);
    }

    private void AddListeners()
    {
        _adMobRewardedVideoAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        _adMobRewardedVideoAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        _adMobRewardedVideoAd.OnAdOpening += HandleRewardBasedVideoOpened;
        _adMobRewardedVideoAd.OnAdStarted += HandleRewardBasedVideoStarted;
        _adMobRewardedVideoAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
        _adMobRewardedVideoAd.OnAdClosed += HandleRewardBasedVideoClosed;
        _adMobRewardedVideoAd.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
        
        _adMobInterstitialAd.OnAdLoaded += HandleInterstitialAdLoaded;
        _adMobInterstitialAd.OnAdFailedToLoad += HandleInterstitialAdFailedToLoad;
        _adMobInterstitialAd.OnAdOpening += HandleInterstitialAdOpened;
        _adMobInterstitialAd.OnAdClosed += HandleInterstitialAdClosed;
        _adMobInterstitialAd.OnAdLeavingApplication += HandleInterstitialAdLeavingApplication;
    }

    private void Update()
    {
        if(_adMobInterstitialAd != null && _adMobInterstitialAd.IsLoaded())
            _adMobInterstitialAd.Show();
    }
}
