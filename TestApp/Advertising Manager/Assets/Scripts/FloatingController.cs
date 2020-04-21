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

// Class to handle the creation of the objects in the scene.  It is called
// floating since the objects are not the floor, and not attached to anything..
public class FloatingController : TerrainController
{


	// height is fixed 
	private float ObjectHeight = 5;

	// known prefabs
	public GameObject block;
	public GameObject doorbell;
	public GameObject coin;

	// keep track of what we make.
	private List<GameObject> mObjects = new List<GameObject> ();

	// Reset the level by cleaning up what we have, then build new ones.
	override public void ResetLevel ()
	{
		List<string> script = GameManager.Instance.LevelInfo.FloatingScript;
		foreach (GameObject g in mObjects) {
			Destroy (g);
		}
		mObjects.Clear ();
		BuildObjects (script);

	}

	// Based on the script passed in, build the various objects.
	void BuildObjects (List<string> script)
	{

		// scale the Z so we can convert the position in the script to position.
		float posZScale = (posMaxZ - posMinZ) / script.Count;

		//reverse the script to draw it in the correct orientation.
		script.Reverse ();

		for (int row=0; row < script.Count; row++) {
			char[] data = script [row].ToCharArray ();
			float posXScale = (posMaxX - posMinX) / data.Length;
			float posYScale = (posMaxY - posMinY) / 10f;
			
			for (int col=0; col<data.Length; col++) {
				
				Vector3 pos;
				GameObject obj;

				pos = new Vector3 ((col * posXScale) + (posXScale * .25f),
				                  ObjectHeight * posYScale, (row * posZScale) + (posZScale / 2f));
				obj = null;
				switch (data [col]) {
						
				case '-':
						// this is nothing
					break;
				case 'S': // stalagtite
					// block of solid material to the roof
					pos = new Vector3 (pos.x, pos.y + (16f - ObjectHeight) / 2.1f, (row * posZScale) + (posZScale * 0.025f));
					obj = Instantiate (block, pos, Quaternion.identity) as GameObject;
					obj.transform.localScale = new Vector3 (posXScale * 1.25f, (16f - ObjectHeight) / 2 * (posMaxY / 16f), posZScale * .9f);
					break;
				case 'B':
						// block of solid material
					Debug.Log ("Building " + block.name);
					obj = Instantiate (block, pos, Quaternion.identity) as GameObject;
					obj.transform.localScale = new Vector3 (posXScale, posMaxY / 16f, posZScale);
					break;
				case 'T':
						// door target
					obj = Instantiate (doorbell, pos, doorbell.transform.localRotation) as GameObject;
					break;
				case 'R':
						// robot
						// position on the floor -- remember we reversed the rows....
					int index = script.Count - row - 1;
					int h = GameManager.Instance.LevelInfo.GetFloorHeight (index, col);
					pos = new Vector3 (pos.x + posXScale/4f, h * posYScale + (posYScale / 2f), (row * posZScale) + (posZScale * 0.25f));
					obj = Instantiate (coin, pos, coin.transform.localRotation) as GameObject;
					break;
				default:
					Debug.Log ("Don't know whata " + data [col] + " represents!");
					break;
				}
				if (obj != null) {
					mObjects.Add (obj);
				}
			}
		}
	}
}
