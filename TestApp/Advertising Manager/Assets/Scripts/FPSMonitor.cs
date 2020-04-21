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
using UnityEngine.UI;

// Based on http://wiki.unity3d.com/index.php?title=FramesPerSecond
// This is a little tool to show the FPS on the game scene.
public class FPSMonitor : MonoBehaviour
{

	// Attach this to a GUIText to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.
	
	public  float updateInterval = 1f;
	private float accum = 0; // FPS accumulated over the interval
	private int   frames = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	
	void Start ()
	{
		timeleft = updateInterval;  
	}
	
	void Update ()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if (timeleft <= 0.0) {
			Text t = GetComponent<Text> ();
			// display two fractional digits (f2 format)
			float fps = accum / frames;
			string format = System.String.Format ("{0:F2} FPS", fps);
			t.text = format;
			
			if (fps < 30)
				t.material.color = Color.yellow;
			else 
				if (fps < 10)
				t.material.color = Color.red;
			else
				t.material.color = Color.green;
			//	DebugConsole.Log(format,level);
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}
