using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool enablePerPlacementLoad = false;
    [SerializeField] private bool isPlatformAndroid = true;
    [SerializeField] private int numberOfCoins = 0;
    [SerializeField] private int numberOfTokens = 0;
    [SerializeField] private Text textBox;
    private RewardBasedVideoAd _adMobRewardedVideoAd;
    private InterstitialAd _adMobInterstitialAd;
    private BannerView _adMobBannerView;
    private const string TextNumberOfCoins = "The player has earned ";
    private const string AdMobGameIdAndroid = "ca-app-pub-1272408203130077~9243552470";
    private const string UnityGameIdAndroid = "3483587";
    private const string UnityGameIdApple = "3483586";
    private const string AdMobRewardedVideoAdId = "ca-app-pub-3940256099942544/5224354917";
    private const string AdMobInterstitialAdId = "ca-app-pub-3940256099942544/1033173712";
    private const string AdMobMenuBannerAdId = "ca-app-pub-3940256099942544/6300978111";
    private const string UnityRewardedVideoPlacementId = "rewardedVideo";
    private const string UnityInterstitialPlacementId = "video";
    private const string UnityMenuBannerPlacementId = "MenuBanner";

    public void OnUnityAdsReady (string placementId) {
        // Causes ad to constantly play 
        //Advertisement.Show (placementId);
    }
    
    public void OnUnityAdsDidError (string errorMessage) {
        Debug.LogWarning (errorMessage);
    }
    
    public void OnUnityAdsDidStart (string placementId) {
        Debug.Log ("The ad started playing.");
    }
    
    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        if(showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
            numberOfCoins++;
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
            numberOfCoins--;
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
    }

    private void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        print("HandleRewardBasedVideoLoaded event received");
        _adMobRewardedVideoAd.Show();
    }

    private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);    }

    private void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        print("HandleRewardBasedVideoOpened event received");
    }

    private void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        print("HandleRewardBasedVideoStarted event received");
    }
    
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        print("HandleRewardBasedVideoRewarded event received for " + amount + " " + type);
        numberOfTokens++;
    }

    private void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        print("HandleRewardBasedVideoClosed event received");
    }

    private void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        print("HandleRewardBasedVideoLeftApplication event received");
    }

    public void AdMobEnableRewardedVideoAd()
    {
        AdRequest request = new AdRequest.Builder().Build();
        _adMobRewardedVideoAd.LoadAd(request, AdMobRewardedVideoAdId);
    }

    private void HandleInterstitialAdLoaded(object sender, EventArgs args)
    {
        print("HandleAdLoaded event received");
        _adMobInterstitialAd.Show();
    }

    private void HandleInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    private void HandleInterstitialAdOpened(object sender, EventArgs args)
    {
        print("HandleAdOpened event received");
    }

    private void HandleInterstitialAdClosed(object sender, EventArgs args)
    {
        print("HandleAdClosed event received");
        //_adMobInterstitialAd.Destroy();
    }

    private void HandleInterstitialAdLeavingApplication(object sender, EventArgs args)
    {
        print("HandleAdLeavingApplication event received");
        //_adMobInterstitialAd.Destroy();
    }

    public void AdMobEnableInterstitialAd()
    {
        _adMobInterstitialAd = new InterstitialAd(AdMobInterstitialAdId);
        AdRequest request = new AdRequest.Builder().Build();
        _adMobInterstitialAd.LoadAd(request);
    }

    public void AdMobEnableBannerAd()
    {
        _adMobBannerView = new BannerView(AdMobMenuBannerAdId, AdSize.Banner, AdPosition.Top);
        AdRequest request = new AdRequest.Builder().Build();
        _adMobBannerView.LoadAd(request);
    }

    public void UnityEnableRewardedVideoAd()
    {
        Advertisement.Load(UnityRewardedVideoPlacementId);
        Advertisement.Show(UnityRewardedVideoPlacementId);
    }
    
    public void UnityEnableInterstitialAd()
    {
        Advertisement.Load(UnityInterstitialPlacementId);
        Advertisement.Show(UnityInterstitialPlacementId);
    }
    
    public void UnityEnableBannerAd()
    {
        Advertisement.Banner.Load();
        Advertisement.Banner.Show(UnityMenuBannerPlacementId);
    }

    private void UpdateTextField()
    {
        if (textBox)
        {
            textBox.text = TextNumberOfCoins + "" + numberOfCoins + " coins and " + numberOfTokens + " tokens";
        }
        else
        {
            Debug.LogError("Text box object not set");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        string gameId = isPlatformAndroid ? UnityGameIdAndroid : UnityGameIdApple;
        Advertisement.AddListener(GetComponent<IUnityAdsListener>());
        Advertisement.Initialize(gameId, testMode, enablePerPlacementLoad);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        UpdateTextField();
        _adMobRewardedVideoAd = RewardBasedVideoAd.Instance;
        AddListeners();
        MobileAds.Initialize(AdMobGameIdAndroid);
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
        UpdateTextField();
        
        if(_adMobInterstitialAd.IsLoaded())
            _adMobInterstitialAd.Show();
    }
}
