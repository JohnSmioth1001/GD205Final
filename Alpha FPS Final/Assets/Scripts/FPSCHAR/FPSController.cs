using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//this is the First Person Controller Script that is used for the character in The Multiplayer FPS Game that I have made.
// Most of the code that i got was from Awesome Tuts 
//FPS Tutorial on Udemy

public class FPSController : NetworkBehaviour {
   // everything that is public are things that can be changed in the inspector
    private Transform firstPerson_View;
    private Transform firstPerson_Camera;

    private Vector3 firstPerson_View_Rotation = Vector3.zero;

    //the 5 public floats below have to do with the characters and movement
    public float walkSpeed = 6.75f;
    public float runSpeed = 10f;
    public float crouchSpeed = 4f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    private float speed;

    private bool is_Moving, is_Grounded, is_Crouching;

    private float InputX, InputY;
    private float InputX_Set, InputY_Set;
    private float InputModifyFactor;

    private bool limitDiagonalSpeed = true;

    private float antiBumpFactor = 0.75f;

    private CharacterController charController;
    private Vector3 moveDirection = Vector3.zero;

    
    //focusing on Crouch Running and Jumping
    public LayerMask groundLayer;
    private float rayDistance;
    private float default_ControllerHeight;
    private Vector3 default_CamPos;
    private float camHeight;

    //calling the player animations script into this script
    private FPSPlayerAnimations playerAnimation;

    //calling the weapons game objects that i will call in the inspector
    [SerializeField]
    private WeaponManager weapon_Manager;
    private FPSWeapon current_Weapon;

    //the fire rate and the amount of time between the shots
    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private WeaponManager handweapon_Manager;
    private FPSHandsWeapon current_Hands_Weapon;

    public GameObject playerHolder, weaponsHolder;
    public GameObject[] weapons_FPS;
    private Camera mainCam;
    public FPSMouseLook[] mouseLookLikeThis;

    //both private Color and public renderer, are placed there to make the enemy look different from the player
    private Color[] playerColors = new Color[] { new Color(0, 44, 255, 255), new Color(252, 208, 193, 255), new Color(0, 0, 0, 255) };

    public Renderer playerRenderer;

	// Use this for initialization
	void Start () {
        //The start function in the beginning is to call all of the components that we need.
        
        firstPerson_View = transform.Find("FPS View").transform;
        charController = GetComponent<CharacterController>();
        speed = walkSpeed;
        is_Moving = false;
        //video three 
        rayDistance = charController.height * 0.5f + charController.radius;
        default_ControllerHeight = charController.height;
        default_CamPos = firstPerson_View.localPosition;

        playerAnimation = GetComponent<FPSPlayerAnimations>();
        
        //down below is the parts for calling the player's weapons and showing them in the game

        weapon_Manager.weapons[0].SetActive(true);
        current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();

        handweapon_Manager.weapons[0].SetActive(true);
        current_Hands_Weapon = handweapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();

        //both of the if statements are calling the layer mask of the local player and client player on the server
        //if you are the local player than Put the player at this layer.
        if (isLocalPlayer)
        {
            playerHolder.layer = LayerMask.NameToLayer("Player");

            foreach (Transform child in playerHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }

            for (int i = 0; i < weapons_FPS.Length; i++)
            {
                weapons_FPS[i].layer = LayerMask.NameToLayer("Player");
            }

            weaponsHolder.layer = LayerMask.NameToLayer("Enemy");

            foreach(Transform child in weaponsHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }

        }

        //if you are not the local player then put the player on the enemy layer
        if (!isLocalPlayer)
        {
            playerHolder.layer = LayerMask.NameToLayer("Enemy");

            foreach (Transform child in playerHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }

            for (int i = 0; i < weapons_FPS.Length; i++)
            {
                weapons_FPS[i].layer = LayerMask.NameToLayer("Enemy");
            }

            weaponsHolder.layer = LayerMask.NameToLayer("Player");

            foreach (Transform child in weaponsHolder.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Player");
            }

        }

        if (!isLocalPlayer)
        {
            for (int i = 0; i < mouseLookLikeThis.Length; i++)
            {
                mouseLookLikeThis[i].enabled = false;
            }


        }

        mainCam = transform.Find("FPS View").Find("FPS Camera").GetComponent<Camera>();
        mainCam.gameObject.SetActive(false);

        //this is changing the color of the client player and making it appear different for the player
        //so any enemy will look blue while you will look red. But vice versa as well on a different computer
        if (!isLocalPlayer)
        {
            for (int i = 0; i < playerRenderer.materials.Length; i++)
            {
                playerRenderer.materials[i].color = playerColors[i];
            }
        }

    }

    //This is looking for the player tag that is placed on the character
    public override void OnStartLocalPlayer()
    {
        tag = "Player";

    }

    // Update is called once per frame
    void Update () {
        //turning on the camera since there is no camera displayed in the scene.
        //this camera will be the player's camera
        if (isLocalPlayer)
        {
            if (!mainCam.gameObject.activeInHierarchy)
            {
                mainCam.gameObject.SetActive(true);
            }

        }


        if (!isLocalPlayer)
        {
            return;
        }


        //Allowing the player movement and the ability to switch weapons 
        PlayerMovement();
        SelectWeapon();
	}

    //First movement video
    //This is just for Player movement
    void PlayerMovement()

        //All of this code is for the player to move using the keys w, a,s,d
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                InputY_Set = 1f;
            }
            else
            {
                InputY_Set = -1f;
            }


        }
        else
        {
            InputY_Set = 0f;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                InputX_Set = -1f;
            }
            else
            {
                InputX_Set = 1;
            }

        }
        else
        {
            InputX_Set = 0f;
        }

        InputY = Mathf.Lerp(InputY, InputY_Set, Time.deltaTime * 19f);
        InputX = Mathf.Lerp(InputX, InputX_Set, Time.deltaTime * 19f);

        InputModifyFactor = Mathf.Lerp(InputModifyFactor,
            (InputY_Set != 0 && InputX_Set != 0 && limitDiagonalSpeed ? 0.75f : 1.5f),
            Time.deltaTime * 19f);

        firstPerson_View_Rotation = Vector3.Lerp(firstPerson_View_Rotation,
            Vector3.zero, Time.deltaTime * 5f);
        firstPerson_View.localEulerAngles = firstPerson_View_Rotation;

        //this is making sure that if the player is on the ground layer that they can move
        if (is_Grounded)
        {
            //here we are going to call the crouch and sprint 
            PlayerCrouchSprint();

            moveDirection = new Vector3(InputX * InputModifyFactor, -antiBumpFactor, InputY * InputModifyFactor);

            moveDirection = transform.TransformDirection (moveDirection) * speed;

            //here we are going to call the jump
            PlayerJump();
        }

        moveDirection.y -= gravity * Time.deltaTime;

        is_Grounded = (charController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        is_Moving = charController.velocity.magnitude > 0.15f;

        //calling the player animations that were made for the character instead of being in the T pose
        HandleAnimations();

    }

    //this function is just so the player can crouch as well as sprint
    void PlayerCrouchSprint()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!is_Crouching)
            {
                is_Crouching = true;
            }
            else
            {
                if (CantGetUp ())
                {
                    is_Crouching = false;
                }

            }

            //allows the camera and the animation to crouch instead of the camera moving and the player standing straight up

            StopCoroutine(MoveCameraCrouch());
            StartCoroutine(MoveCameraCrouch());


        }

        if (is_Crouching)
        {
            speed = crouchSpeed;
        }
        else
        {
            if (Input.GetKey (KeyCode.LeftShift))
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }

        }
        playerAnimation.PlayerCrouch(is_Crouching);


    }

    //video three
    bool CantGetUp()
    {
        Ray groundRay = new Ray(transform.position, transform.up);
        RaycastHit groundHit;

        if (Physics.SphereCast(groundRay, charController.radius + 0.05f, out groundHit, rayDistance, groundLayer))
        {
            if (Vector3.Distance (transform.position, groundHit.point) < 2.3f)
            {
                return false;
            }

        }


        return true;
    }

   //this is calling the camera to move into the crouch position

    IEnumerator MoveCameraCrouch()
    {
        charController.height = is_Crouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;
        charController.center = new Vector3(0f, charController.height / 2f, 0f);

        camHeight = is_Crouching ? default_CamPos.y / 1.5f : default_CamPos.y;

        while (Mathf.Abs (camHeight - firstPerson_View.localPosition.y) > 0.01f)
        {
            firstPerson_View.localPosition = Vector3.Lerp(firstPerson_View.localPosition, 
                new Vector3 (default_CamPos.x, camHeight, default_CamPos.z ), Time.deltaTime * 11f);

            yield return null;
        }
    }


    //this function allows the player to jump
    void PlayerJump()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {

            if (is_Crouching)
            {

                if (CantGetUp ())
                {
                    is_Crouching = false;

                    playerAnimation.PlayerCrouch(is_Crouching);


                    StopCoroutine(MoveCameraCrouch());
                    StartCoroutine(MoveCameraCrouch());

                }

            }

            else
            {
                moveDirection.y = jumpSpeed;
            }

        }


    }
    //this functions is going to call the player animations that were made inside of unity 
    //and allowing them to be shown in the game
    void HandleAnimations()
    {
        playerAnimation.Movement(charController.velocity.magnitude);
        playerAnimation.PlayerJump(charController.velocity.y);

        if (is_Crouching && charController.velocity.magnitude > 0f)
        {
            playerAnimation.PlayerCrouchWalk(charController.velocity.magnitude);
        }

        //Shooting
        if (Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            if (is_Crouching)
            {
                playerAnimation.Shoot(false);
            }
            else
            {
                playerAnimation.Shoot(true);
            }

            current_Weapon.Shoot();
            current_Hands_Weapon.Shoot();
        }

        if (Input.GetKeyDown (KeyCode.R))
        {
            playerAnimation.ReloadGun();
            current_Hands_Weapon.Reload();
        }

    }

    //This long list of repeated code is what gives the player to switch between weapons.
    //the desert eagle is on the 1 key
    //the ak47 is on the 2 key
    //the m4a1 is on the 3 key
    //at any time you can switch between them
    void SelectWeapon()
    {
        if (Input.GetKeyDown (KeyCode.Alpha1))
        {

            if (!handweapon_Manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < handweapon_Manager.weapons.Length; i++)
                {
                    handweapon_Manager.weapons[i].SetActive(false);
                }

                current_Hands_Weapon = null;

                handweapon_Manager.weapons[0].SetActive(true);
                current_Hands_Weapon = handweapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
            }


            if (!weapon_Manager.weapons[0].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[0].SetActive(true);
                current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();

                playerAnimation.ChangeController(true);
            }


        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            if (!handweapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < handweapon_Manager.weapons.Length; i++)
                {
                    handweapon_Manager.weapons[i].SetActive(false);
                }

                current_Hands_Weapon = null;

                handweapon_Manager.weapons[1].SetActive(true);
                current_Hands_Weapon = handweapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();
            }



            if (!weapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[1].SetActive(true);
                current_Weapon = weapon_Manager.weapons[1].GetComponent<FPSWeapon>();

                playerAnimation.ChangeController(false);
            }


        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            if (!handweapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < handweapon_Manager.weapons.Length; i++)
                {
                    handweapon_Manager.weapons[i].SetActive(false);
                }

                current_Hands_Weapon = null;

                handweapon_Manager.weapons[2].SetActive(true);
                current_Hands_Weapon = handweapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();
            }


            if (!weapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[2].SetActive(true);
                current_Weapon = weapon_Manager.weapons[2].GetComponent<FPSWeapon>();

                playerAnimation.ChangeController(false);
            }


        }

    }



}//class
















































