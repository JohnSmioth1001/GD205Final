using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSShootingControl : NetworkBehaviour {
    //This is where the shooting and damage happens
    // below i made thevariables that i will call in the script

    private Camera mainCam;

    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    public GameObject concrete_Impact, blood_Impact;

    public float damageAmount = 5f;

  


    // Use this for initialization
    void Start () {
        //calling for tyhe specific camera that is on the Fps player prefab
        mainCam = transform.Find("FPS View").Find("FPS Camera").GetComponent<Camera>(); ;

	}
	
	// Update is called once per frame
	void Update () {
        //calling the shoot function
        Shoot();
	}

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >  nextTimeToFire )
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            RaycastHit hit;

            if (Physics.Raycast (mainCam.transform.position, mainCam.transform.forward, out hit) )
            {

                if (hit.transform.tag == "Player")
                {
                    CmdDealDamage(hit.transform.gameObject, hit.point, hit.normal);

                }
                else
                {
                    Instantiate(concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
                }

            }

        }

    }

    [Command]
    void CmdDealDamage(GameObject obj, Vector3 pos, Vector3 rotation)
    {
        obj.GetComponent<FPSHealth>().TakeDamage(damageAmount);
        //this line of code below is what is giving me the issue\
        //the players are not getting the impact of the bullets that reflect damage and getting shot.
        //I have had the error pop up in the inspector and not one error code but the damage does not work.
        //no typos, no wrong placement, not forgetting anything but this line of code is what has messed me up.
       Instantiate(blood_Impact, pos, Quaternion.LookRotation (rotation) );
    }

 
}//class





















