using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool enablePerPlacementLoad = false;
    [SerializeField] private bool isPlatformAndroid = true;
    private const string GameIdAndroid = "3483587";
    private const string GameIdApple = "3483586";
    private const string RewardedVideoPlacementId = "rewardedVideo";
    private const string InterstitialPlacementId = "video";
    private const string MenuBannerPlacementId = "MenuBanner";
    
    public void OnUnityAdsReady (string placementId) {
        Advertisement.Show (placementId);
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
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
    }

    public void EnableRewardedVideoAd()
    {
        print("Rewarded Video");
        EnableAd(RewardedVideoPlacementId);
    }
    
    public void EnableInterstitialAd()
    {
        print("Interstitial");
        EnableAd(InterstitialPlacementId);
    }
    
    public void EnableBannerAd()
    {
        print("Banner");
        Advertisement.Banner.Load();
        Advertisement.Banner.Show();
        //EnableAd(MenuBannerPlacementId);
    }

    private void EnableAd(string placementId)
    {
        Advertisement.Load(placementId);
        if (Advertisement.IsReady(placementId))
            Advertisement.Show(placementId);
    }

    // Start is called before the first frame update
    void Start()
    {
        string gameId = isPlatformAndroid ? GameIdAndroid : GameIdApple;
        Advertisement.Initialize(gameId, testMode, enablePerPlacementLoad);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    }
}
