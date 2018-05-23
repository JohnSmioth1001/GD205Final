using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartRespawn : MonoBehaviour {
    public Scene scene;

    //a very simple script that handles the respawn.
    //used it in other games but not working in the fps game

	// Use this for initialization
	void Start () {
        scene = SceneManager.GetActiveScene();
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other) {
		if(other.gameObject.tag == "Enemy")
        {
           SceneManager.LoadScene(scene.name);
        }
   
	}
}
