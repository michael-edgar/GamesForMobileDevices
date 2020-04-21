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

// Handles moving the player along the path in the level.
public class Movement : MonoBehaviour
{

	public GameObject cameraObj;
	public float acceleration = 500f;
	public TerrainController terrainController;
	public float maxSpeed = 20;
	private Vector2 horizontalMovement;
	private float deceleration = 0;
	private float decelerationX;
	private float decelerationZ;
	private List<Vector3> path;
	private int current;
	private Vector3 heading;
	private float lastDistance;

	void Start ()
	{
		StartLevel ();
	}

	public void StartLevel ()
	{
		terrainController.ResetLevel ();
		List<string> script = GameManager.Instance.LevelInfo.FloatingScript;
		path = ReadPath (script);
		transform.position = path [0];
		current = 0;
		lastDistance = 0;
	}


	// Update is called once per frame
	void FixedUpdate ()
	{

		Vector3 vel = new Vector3 (GetComponent<Rigidbody>().velocity.x, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);



		bool manual = false;
		if (manual) {

			if (vel.magnitude > maxSpeed) {
				vel.Normalize ();
				vel *= maxSpeed;
				GetComponent<Rigidbody>().velocity = vel;
			}

			if (Input.GetAxis ("Horizontal") == 0 && Input.GetAxis ("Vertical") == 0) {
				GetComponent<Rigidbody> ().velocity = new Vector3 (
					Mathf.SmoothDamp (GetComponent<Rigidbody> ().velocity.x, 0, ref decelerationX, deceleration),
					GetComponent<Rigidbody> ().velocity.y,
				Mathf.SmoothDamp (GetComponent<Rigidbody> ().velocity.z, 0, ref decelerationZ, deceleration));
			}
			// uncomment to go where you are looking.
			//transform.rotation = Quaternion.Euler(0,cameraObj.GetComponent<MouseLooking>().CurrentYRotation,0);
			GetComponent<Rigidbody> ().AddRelativeForce (Input.GetAxis ("Horizontal") * acceleration * Time.deltaTime, 0,
			                           Input.GetAxis ("Vertical") * acceleration * Time.deltaTime);



		} else {

			if (vel.magnitude < maxSpeed) {
				vel *= maxSpeed;
			}

			if (current >= 0 && current < path.Count) {
				float distance = Vector3.Distance (transform.position, path [current]);
				heading = GetComponent<Rigidbody>().velocity.normalized;
				if (distance < 2f) {
					if (current < path.Count - 1) {
						current++;
					} else {
						current = path.Count - 1;
						GameManager.Instance.ShowEndMenu ();
            
					}
					heading = path [current] - transform.position;
					distance = Vector3.Distance (transform.position, path [current]);
				} else if (lastDistance - distance < -.25f) {
					heading = path [current] - transform.position;
				}
				GetComponent<Rigidbody>().velocity = Vector3.Lerp (GetComponent<Rigidbody>().velocity, (heading.normalized * maxSpeed), 2f * Time.deltaTime);
				lastDistance = Mathf.Min (lastDistance, distance);
			}

		}
		GameManager.Instance.PlayerPosition = transform.position;
	}

	List<Vector3> ReadPath (List<string> script)
	{

		List<Vector3> ret = new List<Vector3> ();
		float posXScale;
		float posZScale = (terrainController.PosMax.z - terrainController.PosMin.z) / script.Count;
		float posYScale = terrainController.PosMax.y / 16f;
		List<string> rev = new List<string> (script);
		rev.Reverse ();

		for (int row=0; row < rev.Count; row++) {
			char[] data = rev [row].ToCharArray ();
			posXScale = (terrainController.PosMax.x - terrainController.PosMin.x) / data.Length;
			for (int col=0; col<data.Length; col++) {
				float ht = terrainController.GetHeightForSymbol (data [col]);
				// ht is a percentage 0 - 1 of the height, skip walls
				if (ht >= 0 && data [col] != '#') {
					float x = posXScale * col + (posXScale * .2f);
					float y = terrainController.PosMax.y * ht + (posYScale * .3f);
					float z = posZScale * row + (posZScale * .3f);
					ret.Add (new Vector3 (x, y, z));
				}
			}
		}
		List<Vector3> sorted = new List<Vector3> ();

		Vector3 lastPoint = ret [0];
		Vector3 nextPt;
		sorted.Add (lastPoint);
		ret.Remove (lastPoint);
		while (ret.Count > 0) {
			float min = 0;
			nextPt = Vector3.zero;
			foreach (Vector3 pt in ret) {
				if (nextPt == Vector3.zero || min > (lastPoint - pt).sqrMagnitude) {
					min = (lastPoint - pt).sqrMagnitude;
					nextPt = pt;
				}
			}
			sorted.Add (nextPt);
			ret.Remove (nextPt);
			lastPoint = nextPt;
      
		}
		// add a point off the map
		if (sorted [sorted.Count - 1].z < terrainController.PosMax.z) {
			sorted.Add (new Vector3 (sorted [sorted.Count - 1].x, sorted [sorted.Count - 1].y,
			                       terrainController.PosMax.z * 0.99f)); 
		}
		return sorted;
    
	}
  
	void OnCollisionEnter (Collision col)
	{
		// uncomment for debugging moving around and bumping into stuff.
		//Debug.Log ("OUCH!! Hit a " + col.gameObject.name);
	}
}
