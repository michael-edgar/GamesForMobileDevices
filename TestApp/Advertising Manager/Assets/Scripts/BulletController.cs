using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public GameObject cube;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	
	}

	void OnCollisionEnter (Collision col)
	{
//		Debug.Log ("BOOM!! " + gameObject.name + "  Hit a " + col.gameObject.name + " at " + transform.position);

		if (col.gameObject.name.Contains("android")) {
			col.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			col.gameObject.GetComponent<Rigidbody>().useGravity = true;
			Debug.Log ("BOOM!! " + gameObject.name + "  Hit a " + col.gameObject.name + " at " + transform.position);

			col.gameObject.GetComponent<DroidController>().Die();

		}
		//if (col.gameObject.name.Equals("Terrain") ){
		////Instantiate(cube,transform.position,transform.rotation);
		//}
	}
}
