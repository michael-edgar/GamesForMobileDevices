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
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;
using UnityEngine.UI;
// Global game state and navigation to menus.
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


	private static GameManager THE_INSTANCE;
	int numLevels = 2;
	private int mLevel;
	private int mHits;
	public GameObject mPlayer;
	private Quaternion mCannonRotation;
	private Vector3 mCannonForward;
	private LevelManager mLevelManager;
	public GameObject mEndMenu;
    public Button mainMenuButton;
    public Button restartMenuButton;
	
	public GameManager ()
	{
		THE_INSTANCE = this;
		mLevel = 0;
		mLevelManager = new LevelManager ();
		mHits = 0;
	}

	// Use this for initialization
	void Awake ()
	{
		DontDestroyOnLoad (gameObject);
	}


    public void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoMainMenu();
        }
    }
	

	public LevelManager LevelInfo {
		get {
			if (mLevelManager.Level != mLevel) {
				mLevelManager.LoadLevel (mLevel);
			}
			return mLevelManager;
		}
	}

	public Vector3 PlayerPosition {
		get {
			return mPlayer.transform.position;
		}
		set {
			mPlayer.transform.position = value;
		}
	}

	public Quaternion CannonRotation {
		get {
			return mCannonRotation;
		}
		set {
			mCannonRotation = value;
		}
	}

	public Vector3 CannonForward {
		get {
			return mCannonForward;
		}
		set {
			mCannonForward = value;
		}
	}

	public int Level {
		get {
			return mLevel;
		}
	}

	public static GameManager Instance {
		get {
			return THE_INSTANCE;
		}
	}

	public void PlayHitSound ()
	{
		mPlayer.GetComponent<AudioSource> ().Play ();
	}

	public bool IsMenuShowing {
		get {
			return mEndMenu.activeSelf;
		}
	}

	public void ShowEndMenu ()
	{
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(restartMenuButton.gameObject);
        }
        if (mEndMenu.activeSelf)
        {
            return;
        }

        else
        {
	        // Submit leaderboard scores, if authenticated
	        if (PlayGamesPlatform.Instance.localUser.authenticated)
	        {
		        PlayGamesPlatform.Instance.ReportScore(mHits, GPGSIds.leaderboard_targets_hit_in_one_level,
			        (bool success) =>
			        {
				        Debug.Log("(Lollygagger) Leaderboard update success: " + success);
			        });
	        }
        }

        WriteUpdatedScore();
        mEndMenu.SetActive (true);
	}

	public void GoMainMenu ()
	{
		Debug.Log("Going Main Menu!");
		FadeController fader = mPlayer.GetComponentInChildren<FadeController>();
		if (fader != null) {
            fader.GetComponent<FadeController>().FadeToLevel(()=>SceneManager.LoadScene ("MainMenu"));
		}
		else {
            SceneManager.LoadScene("MainMenu");
		}

	}

	public void RestartLevel ()
	{
		FadeController fader = mPlayer.GetComponentInChildren<FadeController>();
		if (fader != null) {
			fader.FadeToLevel(()=>{
			mEndMenu.SetActive (false);
			fader.StartScene();
			mPlayer.GetComponent<Movement> ().StartLevel ();
			});
		}
		else {
			mEndMenu.SetActive (false);
			mPlayer.GetComponent<Movement> ().StartLevel ();
		}
    
		mHits = 0;
	}

	public void NextLevel ()
	{
		FadeController fader = mPlayer.GetComponentInChildren<FadeController>();
		if (fader != null) {
			fader.FadeToLevel(()=>{
			mEndMenu.SetActive (false);
			fader.StartScene();
			mLevel = (mLevel + 1) % numLevels;
			mPlayer.GetComponent<Movement> ().StartLevel ();
			});
		}
		else {
			mEndMenu.SetActive (false);
			mLevel = (mLevel + 1) % numLevels;
			mPlayer.GetComponent<Movement> ().StartLevel ();
		}
		
		mHits = 0;
	}

	public void IncrementHits()
	{
		mHits = mHits + 1;
	}

	public void ReadSavedGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, callback);
	}

	public void WriteSavedGame(ISavedGameMetadata game, byte[] savedData,
		Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
	{
		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder().WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1)).WithUpdatedDescription("Saved at: " + System.DateTime.Now);

		SavedGameMetadataUpdate updatedMetadata = builder.Build();

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
	}

	public void WriteUpdatedScore()
	{
		ISavedGameMetadata currentGame = null;

		Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback =
			(SavedGameRequestStatus status, ISavedGameMetadata game) =>
			{
				Debug.Log("(Lollygagger) Saved Game Write: " + status.ToString());
			};

		Action<SavedGameRequestStatus, byte[]> readBinaryCallback = (SavedGameRequestStatus status, byte[] data) =>
		{
			Debug.Log("(Lollygagger) Saved Game Binary Read: " + status.ToString());
			if (status == SavedGameRequestStatus.Success)
			{
				// Read score from the Saved Game
				int score = 0;
				try
				{
					string scoreString = System.Text.Encoding.UTF8.GetString(data);
					score = Convert.ToInt32(scoreString);
				}
				catch (Exception e)
				{
					Debug.Log("(Lollygagger) Saved Game Write: convert exception");
				}
				
				// Increment score, convert to byte[]
				int newScore = score + mHits;
				string newScoreString = Convert.ToString(newScore);
				byte[] newData = System.Text.Encoding.UTF8.GetBytes(newScoreString);
				
				// Write new data
				Debug.Log("(Lollygagger) Old Score: " + score.ToString());
				Debug.Log("(Lollygagger) mHits: " + mHits.ToString());
				Debug.Log("(Lollygagger) New Score: " + newScore.ToString());
				WriteSavedGame(currentGame, newData, writeCallback);
			}
		};
		
		// CALLBACK: Handle the result of a read, which should return metadata
		Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback = 
			(SavedGameRequestStatus status, ISavedGameMetadata game) => {
				Debug.Log("(Lollygagger) Saved Game Read: " + status.ToString());
				if (status == SavedGameRequestStatus.Success) {
					// Read the binary game data
					currentGame = game;
					PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, 
						readBinaryCallback);
				}
			};

		// Read the current data and kick off the callback chain
		Debug.Log("(Lollygagger) Saved Game: Reading");
		ReadSavedGame("file_total_hits", readCallback);
	}
}
