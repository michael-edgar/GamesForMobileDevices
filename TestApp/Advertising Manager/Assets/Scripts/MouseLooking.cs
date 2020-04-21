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

// Handles using the mouse to look around.
public class MouseLooking : MonoBehaviour
{

	private float lookSensitivity = 5f;
	private float yRotation;
	private float xRotation;
	private float mCurrentXRotation;
	private float mCurrentYRotation;
	private float yRotationV;
	private float xRotationV;
	private float smoothness = 0.1f;

	
	// Update is called once per frame
	void Update ()
	{

		if (GameManager.Instance.IsMenuShowing) {
			return;
		}


		if (Input.touches.Length > 0) {

			for (int i=0; i<Input.touches.Length; i++) {
				Touch t = Input.touches [i];
				if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Ended) {
					return;
				}
			}

			lookSensitivity = 2f;
		} else {
			lookSensitivity = 5f;
		}

		yRotation += Input.GetAxis ("Mouse X") * lookSensitivity;
		xRotation -= Input.GetAxis ("Mouse Y") * lookSensitivity;

		xRotation = Mathf.Clamp (xRotation, -80, 100);

		mCurrentXRotation = Mathf.SmoothDamp (mCurrentXRotation, xRotation, ref xRotationV, smoothness);
		mCurrentYRotation = Mathf.SmoothDamp (mCurrentYRotation, yRotation, ref yRotationV, smoothness);
		
		transform.rotation = Quaternion.Euler (xRotation, yRotation, 0);
	}

	public float CurrentYRotation {
		get {
			return mCurrentYRotation;
		}
	}

	public float XRotation {
		get {
			return xRotation;
		}
	}

	public float YRotation {
		get {
			return yRotation;
		}
	}
}
