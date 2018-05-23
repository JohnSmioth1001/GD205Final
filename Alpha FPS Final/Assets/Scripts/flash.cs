using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flash : MonoBehaviour {

	public float min;
	public float max;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		GetComponent<Light> ().intensity = Mathf.PingPong (Time.time * 100f, max - min) + min;

		//GetComponent<Light> ().intensity=Mathf.PingPong(Time.time,20f);

		//GetComponent<Light> ().range=Mathf.PingPong(Time.time,20f);

	}
}
