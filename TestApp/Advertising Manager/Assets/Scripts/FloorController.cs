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
using System;
using System.Collections.Generic;
using System.IO;

// Controls the generation of the floor of the scene.
public class FloorController : TerrainController
{
	
	public GameObject cubemesh;
	public GameObject fog;
	public GameObject door;
	private int currentLevel = -1;
	private List<GameObject> ldoors = new List<GameObject> ();
	private List<GameObject> rdoors = new List<GameObject> ();

	// Cleans up the existing objects and makes new ones.
	override public void ResetLevel ()
	{

		if (LevelInfo.Level != currentLevel) {
			foreach (GameObject g in ldoors) {
				Destroy (g);
			}
			foreach (GameObject g in rdoors) {
				Destroy (g);
			}
			ldoors.Clear ();
			rdoors.Clear ();

			List<string> mScript = LevelInfo.FloorScript;
			reset ();
			BuildSceneFromScript (mScript);
			gameObject.GetComponent<FloatingController> ().ResetLevel ();
			currentLevel = LevelInfo.Level;
		}
	}

	// Build the terrain from the provided script.
	void BuildSceneFromScript (List<string> script)
	{
		
		float posZScale = (posMaxZ - posMinZ) / (script.Count);
		float zscale = (1.0f * terrainZRes) / (script.Count);
		float[,] heights = TerrainHeights;
		
		//reverse the script to draw it in the correct orientation.
		script.Reverse ();

		for (int row=0; row < script.Count; row++) {
			char[] data = script [row].ToCharArray ();
			float posXScale = (posMaxX - posMinX) / data.Length;
			
			float xscale = (1.0f * terrainZRes) / data.Length;
			
			for (int col=0; col<data.Length; col++) {
				int x0 = (int)Math.Round (col * xscale);
				int y0 = (int)Math.Round (row * zscale);
				int x1 = (int)Math.Round ((col + 1) * xscale);
				int y1 = (int)Math.Round ((row + 1) * zscale);
				float h = -1f;
				
				h = GetHeightForSymbol (data [col]);
				

				if (h < 0) {
					BuildObjects (data [col], row, col, posXScale, posZScale);
				}
				if (h > 0) {
					BuildCubeMesh (data [col], row, col, h, posXScale, posZScale);
					for (int y= y0; y < y1; y++) {
						for (int x=x0; x<x1; x++) {
							heights [y, x] = h;
						}
					}
				}
				
			}
		}
		TerrainHeights = heights;
		
	}
	
	// We have to put a block around the parts of the floor, this makes the physics of the floor better than the
	// imperfect terrain collider. 
	void BuildCubeMesh (char symbol, int row, int col, float height, float posXScale, float posZScale)
	{
		Vector3 pos;
		GameObject obj;
		float posYScale = 20 * (posMaxY - posMinY) / 10f;
		
		if (height == 0) {
			return;
		}

		// Add a cube-y collision mesh
		pos = new Vector3 ((col * posXScale) + (posXScale / 2f), 0, (row * posZScale) + (posZScale / 2));
		obj = Instantiate (cubemesh, pos, Quaternion.identity) as GameObject;
		obj.transform.localScale = new Vector3 (posXScale + posMaxX / terrainXRes, 
		                                        height * posYScale, 
		                                        posZScale + posMaxZ / terrainZRes);
		// Hide the box itself
		obj.GetComponent<MeshRenderer> ().enabled = false;
	}

	// Create non floor objects (fog, doors)
	void BuildObjects (char symbol, int row, int col, float posXScale, float posZScale)
	{
		Vector3 pos;
		GameObject obj;
		
		switch (symbol) {
		case '-':
			// the position is the center of the object, so move over 1/2
			pos = new Vector3 ((col * posXScale) + (posXScale / 2f), 0, (row * posZScale) + (posZScale / 2f));
			obj = Instantiate (fog, pos, Quaternion.identity) as GameObject;
			break;
		case 'g':
			pos = new Vector3 ((col * posXScale) - (posXScale * .5f), posMaxY / 3f, (row * posZScale) + (posZScale * .6f));
			obj = Instantiate (door, pos, door.transform.rotation) as GameObject;
			obj.transform.localScale = new Vector3 (1, 1, posMaxY / 16f);
			ldoors.Add (obj);
			break;
		case 'G':
			pos = new Vector3 ((col * posXScale) + (posXScale * .5f), posMaxY / 3f, (row * posZScale) + (posZScale * .6f));
			obj = Instantiate (door, pos, door.transform.rotation) as GameObject;
			obj.transform.localScale = new Vector3 (1, 1, posMaxY / 16f);
			rdoors.Add (obj);
			break;
		default:
			break;
		}
		
	}

	// Open the sliding doors!
	public void OpenDoors ()
	{
		lopened = new Vector3 (posMinX, ldoors [0].transform.position.y, ldoors [0].transform.position.z);
		ropened = new Vector3 (posMaxX, rdoors [0].transform.position.y, rdoors [0].transform.position.z);
		opening = true;                 
	}
	
	private Vector3 lopened;
	private Vector3 ropened;
	private bool opening = false;
	
	void Update ()
	{
		
		// if we are opening, lerp each of the doors to their final destination.
		if (opening) {
			foreach (GameObject d in ldoors) {
				d.transform.position = Vector3.Lerp (d.transform.position, lopened, .75f * Time.deltaTime);
			}
			foreach (GameObject d in rdoors) {
				d.transform.position = Vector3.Lerp (d.transform.position, ropened, .75f * Time.deltaTime);
			}
		}

		// when we reach the end, open the doors.
		else if (GameManager.Instance.PlayerPosition.z >= posMaxZ * .88f) {
			OpenDoors ();
		}
	}
}
