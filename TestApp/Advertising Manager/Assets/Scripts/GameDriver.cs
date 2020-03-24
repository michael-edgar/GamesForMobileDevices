﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDriver : MonoBehaviour
{
    [SerializeField] private bool testMode = true;
    [SerializeField] private bool enablePerPlacementLoad = false;
    [SerializeField] private bool isPlatformAndroid = true;
    private static int _numberOfCoins = 0;
    private static int _numberOfTokens = 0;
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
    [SerializeField] private Text textBox;
    private AdManager _myManager;

    public static void EarnACoin()
    {
        _numberOfCoins++;
    }
    
    public static void LoseACoin()
    {
        _numberOfCoins--;
    }

    public static void EarnAToken()
    {
        _numberOfTokens++;
    }

    private void UpdateTextField()
    {
        if (textBox)
        {
            textBox.text = TextNumberOfCoins + "" + _numberOfCoins + " coins and " + _numberOfTokens + " tokens";
        }
        else
        {
            Debug.LogError("Text box object not set");
        }
    }

    public static void SendMessage(string message, bool isError)
    {
        if (isError)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        string unityGameId = isPlatformAndroid ? UnityGameIdAndroid : UnityGameIdApple;
        _myManager = GetComponent<AdManager>();
        _myManager.SetTestMode(testMode);
        _myManager.SetEnablePerPlacementLoad(enablePerPlacementLoad);
        _myManager.SetAdMobGameId(AdMobGameIdAndroid);
        _myManager.SetUnityGameId(unityGameId);
        _myManager.SetAdMobRewardedVideoId(AdMobRewardedVideoAdId);
        _myManager.SetAdMobInterstitialAdId(AdMobInterstitialAdId);
        _myManager.SetAdMobMenuBannerAdId(AdMobMenuBannerAdId);
        _myManager.SetUnityRewardedVideoPlacementId(UnityRewardedVideoPlacementId);
        _myManager.SetUnityInterstitialPlacementId(UnityInterstitialPlacementId);
        _myManager.SetUnityMenuBannerPlacementId(UnityMenuBannerPlacementId);
        _myManager.SetUpAdvertising();
        UpdateTextField();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextField();
    }
}