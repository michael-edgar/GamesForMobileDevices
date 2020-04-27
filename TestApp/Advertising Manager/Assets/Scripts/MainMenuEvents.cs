/*
 * Copyright (C) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Mainmenu events.
using UnityEngine.SceneManagement;


public class MainMenuEvents : MonoBehaviour
{
	private Text signInButtonText;
	private Text authStatus;
	private GameObject achButton;
	private GameObject ldrButton;
	public float fadeSpeed = 1.5f; 
	public Image mFader;
	bool toBlack = false;
	bool toClear = true;

    void Awake() {
        mFader.color = Color.black;
        mFader.gameObject.SetActive(true);
        toClear = true;
    }

	void Start()
	{
		signInButtonText = GameObject.Find("signInButton").GetComponentInChildren<Text>();
		authStatus = GameObject.Find("authStatus").GetComponent<Text>();
		achButton = GameObject.Find("achButton");
		ldrButton = GameObject.Find("ldrButton");
		
        GameObject startButton = GameObject.Find("startButton");
        EventSystem.current.firstSelectedGameObject = startButton;

        // Create client configuration
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        
        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;
        
        // Initialise and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
	}

	public void SignIn()
	{
		if (!PlayGamesPlatform.Instance.localUser.authenticated)
		{
			// Sign in with Play Game Services, showing the consent dialog by setting the second parameter to isSilent = false.
			PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
		}
		else
		{
			// Sign out of play games
			PlayGamesPlatform.Instance.SignOut();
			
			// Reset UI
			signInButtonText.text = "Sign In";
			authStatus.text = "";
		}
	}

	public void SignInCallback(bool success)
	{
		if (success)
		{
			Debug.Log("(Lollygagger) Signed in!");
			
			// Change sign-in button text
			signInButtonText.text = "Sign out";
			
			// Show the user's name
			authStatus.text = "Signed in as: " + Social.localUser.userName;
		}
		else
		{
			Debug.Log("(Lollygagger) Sign-in failed...");
			
			// Show failure message
			signInButtonText.text = "Sign in";
			authStatus.text = "Sign-in failed";
		}
	}

	public void ShowAchievements()
	{
		if (PlayGamesPlatform.Instance.localUser.authenticated)
		{
			PlayGamesPlatform.Instance.ShowAchievementsUI();
		}
		else
		{
			Debug.Log("Cannot show Achievements, not logged in");
		}
	}

	public void ShowLeaderboards()
	{
		if (PlayGamesPlatform.Instance.localUser.authenticated)
		{
			PlayGamesPlatform.Instance.ShowLeaderboardUI();
		}
		else
		{
			Debug.Log("Cannot show leaderboard: not authenticated");
		}
	}

	public void StartAdManager()
	{
		Debug.Log("Time to start AdManager");
		toBlack = true;
		// Make sure the texture is enabled.
		mFader.gameObject.SetActive(true);
		
		// Start fading towards black.
		FadeToBlack();

		FadeController fader = gameObject.GetComponentInChildren<FadeController>();
		if (fader != null) {
			fader.FadeToLevel(()=>SceneManager.LoadScene("SampleScene"));
		}
		else {
			SceneManager.LoadScene("SampleScene");
		}
	}
	
    void LateUpdate()
    {
        if (toBlack) {
            FadeToBlack();
        }
        else if (toClear) {
            FadeToClear();
        }
    }

    private void Update()
    {
	    achButton.SetActive(Social.localUser.authenticated);
	    ldrButton.SetActive(Social.localUser.authenticated);
    }

    public void Play ()
	{
		Debug.Log ("Playing!!");
		toBlack = true;
		// Make sure the texture is enabled.
		mFader.gameObject.SetActive(true);
		
		// Start fading towards black.
		FadeToBlack();

		FadeController fader = gameObject.GetComponentInChildren<FadeController>();
		if (fader != null) {
			fader.FadeToLevel(()=>SceneManager.LoadScene("GameScene"));
		}
		else {
            SceneManager.LoadScene("GameScene");
		}
		

	}

	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and black.
		mFader.color = Color.Lerp(mFader.color, Color.clear, fadeSpeed * Time.deltaTime);
		// If the screen is almost black...
		if(mFader.color.a <= 0.05f) {

			toClear = false;
			mFader.gameObject.SetActive(false);
			
		}
	}

	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		mFader.color = Color.Lerp(mFader.color, Color.black, fadeSpeed * Time.deltaTime);
		// If the screen is almost black...
		if(mFader.color.a >= 0.95f) {
			// ... reload the level.
			toBlack = false;

		}
	}


}
