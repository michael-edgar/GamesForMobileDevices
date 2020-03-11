using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool enablePerPlacementLoad = false;
    [SerializeField] private bool isPlatformAndroid = true;
    [SerializeField] private int numberOfCoins = 0;
    [SerializeField] private Text textBox;
    private const string TextNumberOfCoins = "The player has earned ";
    private const string GameIdAndroid = "3483587";
    private const string GameIdApple = "3483586";
    private const string RewardedVideoPlacementId = "rewardedVideo";
    private const string InterstitialPlacementId = "video";
    private const string MenuBannerPlacementId = "MenuBanner";

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

    public void EnableRewardedVideoAd()
    {
        Advertisement.Load(RewardedVideoPlacementId);
        Advertisement.Show(RewardedVideoPlacementId);
    }
    
    public void EnableInterstitialAd()
    {
        Advertisement.Load(InterstitialPlacementId);
        Advertisement.Show(InterstitialPlacementId);
    }
    
    public void EnableBannerAd()
    {
        Advertisement.Banner.Load();
        Advertisement.Banner.Show(MenuBannerPlacementId);
    }

    private void UpdateTextField()
    {
        if (textBox) { textBox.text = TextNumberOfCoins + "" + numberOfCoins + " coins"; }
        else { Debug.LogError("Text box object not set"); }
    }

    // Start is called before the first frame update
    void Start()
    {
        string gameId = isPlatformAndroid ? GameIdAndroid : GameIdApple;
        Advertisement.AddListener(GetComponent<IUnityAdsListener>());
        Advertisement.Initialize(gameId, testMode, enablePerPlacementLoad);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        UpdateTextField();
    }

    private void Update()
    {
        UpdateTextField();
    }
}
