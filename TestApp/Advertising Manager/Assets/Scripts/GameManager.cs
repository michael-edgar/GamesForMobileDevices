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
	}

}
