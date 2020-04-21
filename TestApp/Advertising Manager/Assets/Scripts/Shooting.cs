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

// Handles shooting the projectiles.
public class Shooting : MonoBehaviour
{

	private float waitTilFire;
	private float aimSpeed;
	private float fireRate = 1f;
	public float bulletSpeed = 5000f;
	public GameObject bullet;
	public GameObject spawnBullet;
	public GameObject cameraObject;
	int shot = 1;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetButtonUp ("Fire1") && ! GameManager.Instance.IsMenuShowing) {
			if (waitTilFire <= 0) {
				if (bullet) {
					GameObject obj = Instantiate (bullet, 
					                             spawnBullet.transform.position,
					                             spawnBullet.transform.localRotation) as GameObject;


					obj.name = obj.name + "-" + shot;
					shot++;
					Physics.IgnoreCollision (transform.root.GetComponent<Collider>(), obj.transform.GetComponent<Collider>());
					foreach (Collider c in transform.root.gameObject.GetComponentsInChildren<Collider>()) {
						Physics.IgnoreCollision (c, obj.transform.GetComponent<Collider>());
					}
					obj.GetComponent<Rigidbody>().velocity = transform.root.GetComponent<Rigidbody>().velocity +
						(cameraObject.transform.forward * bulletSpeed);
					Destroy (obj, 5f);
					GetComponent<AudioSource> ().Play ();
				}
				waitTilFire = .5f;
			}
		}
		waitTilFire -= Time.deltaTime * fireRate;
	}
}
