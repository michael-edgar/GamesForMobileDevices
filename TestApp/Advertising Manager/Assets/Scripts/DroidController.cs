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

// Controls the Android "targets".
public class DroidController : MonoBehaviour
{

	private float tm;
	private float interval = 1f;
	public bool isDead;

	// Use this for initialization
	void Start ()
	{
		isDead = false;
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (isDead) {
			return;
		}

		// only rotate every so often, need to save those CPU cycles...
		if (tm < interval) {
			//not yet...
			tm += Time.deltaTime;
		}
		tm = 0;

		//find the vector pointing from our position to the target
		Vector3 d = (transform.position - GameManager.Instance.PlayerPosition).normalized;
		
		//create the rotation we need to be in to look at the target
		Quaternion rot = Quaternion.LookRotation (d);
		
		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime * 5f);
	}

	// Handle collisions with other rigidbodies.
	void OnCollisionEnter (Collision col)
	{

		// if we are alive and hit by a projectile, then die.
		// and turn off the constraint to stand up so we bounce around.
		if (!isDead && col.gameObject.tag.Equals ("Projectile")) {
			Rigidbody rb = GetComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.None;
			rb.useGravity = true;
			Die ();
		}
	}

	// When I get hit, I should be in dead mode.
	public void Die ()
	{
		isDead = true;
		// destroy the object after 5 seconds.
		DestroyObject (gameObject, 5f);
		GetComponent<AudioSource> ().Play ();
	}
}
