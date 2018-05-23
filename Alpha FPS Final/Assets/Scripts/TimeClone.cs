using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeClone : MonoBehaviour {

    public GameObject blueprint;
    public float cloneTimer = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //If time has surpassed the cloneTimer then ...
		if(Time.time > cloneTimer)
        {
            //When the cloneTimer runs out the game will not explode 50 enemies immeditely
            cloneTimer += cloneTimer;
            //checking to make sure it works
            Debug.Log("It Works");
            //this will create a enemy when the timer has passed cloneTimer
            Instantiate(blueprint, transform.position, Quaternion.identity);
           
        }
	}
}
