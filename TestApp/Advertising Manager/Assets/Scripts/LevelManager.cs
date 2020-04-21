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
using System.Collections;
using System.Collections.Generic;
using System;

// Reads the level information and keeps track of the various scripts.
public class LevelManager
{

	private int mLevel = -1;
	private List<string> mFloorScript;
	private List<string> mCeilingScript;
	private List<string> mFloatingScript;

	public void LoadLevel (int level)
	{

		mFloorScript = new List<string> ();
		mCeilingScript = new List<string> ();
		mFloatingScript = new List<string> ();
		mLevel = level;
		Debug.Log ("**** Loading Level " + mLevel);
		TextAsset ta = (TextAsset)Resources.Load ("gunner" + mLevel);
		string[] lines = ta.text.Replace ("\r", "").Split (new char[] { '\n' });
		string section = null;
		foreach (string l in lines) {
			string line = l.Trim ();
			if (line.Length > 0 && !line.StartsWith ("!")) {
				// check for section
				if (line.Equals ("[FLOOR]")) {
					section = line;
				} else if (line.Equals ("[CEILING]")) {
					section = line;
				} else if (line.Equals ("[FLOATING]")) {
					section = line;
				} else if (section == null) {
					throw new UnityException ("Syntax error at " + line + ", unset section");
				} else {
					if (section.Equals ("[FLOOR]")) {
						mFloorScript.Add (line);
					} else if (section.Equals ("[CEILING]")) {
						mCeilingScript.Add (line);
					} else if (section.Equals ("[FLOATING]")) {
						mFloatingScript.Add (line);
					}
				}
			}
		}

	}

	public int GetFloorHeight (int row, int col)
	{
		List<string> floor = FloorScript;
		if (floor.Count > row) {
			string data = floor [row];
			if (data.Length > col) {
				char c = data [col];
				if (c >= '0' && c <= '9') {
					return Convert.ToInt16 (c) - 48;
				}
			}
		}
		return 0;
	}

	public List<string> FloorScript {
		get {
			List<string> ret = new List<string> ();
			ret.AddRange (mFloorScript);
			return  ret;
		}
	}

	public List<string> CeilingScript {
		get {
			List<string> ret = new List<string> ();
			ret.AddRange (mCeilingScript);
			return  ret;
		}
	}

	public List<string> FloatingScript {
		get {
			List<string> ret = new List<string> ();
			ret.AddRange (mFloatingScript);
			return  ret;
		}
	}

	public int Level {
		get {
			return mLevel;
		}
	}


}
