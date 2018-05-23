﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FPSPlayerAnimations : NetworkBehaviour {

    private Animator anim;

    private string MOVE = "Move";
    private string VELOCITY_Y = "VelocityY";
    private string CROUCH_WALK = "CrouchWalk";
    private string CROUCH = "Crouch";

    private string STAND_SHOOT = "StandShoot";
    private string CROUCH_SHOOT = "CrouchShoot";
    private string RELOAD = "Reload";

    public RuntimeAnimatorController animController_Pistol, animController_MachineGun;

    private NetworkAnimator networkAnim;

	// Use this for initialization
	void Awake () {

        anim = GetComponent<Animator>();
        networkAnim = GetComponent<NetworkAnimator>();

	}
	
	public void Movement(float magnitude)
    {
        anim.SetFloat(MOVE, magnitude);
    }

    public void PlayerJump(float velocity)
    {
        anim.SetFloat(VELOCITY_Y, velocity);
    }

    public void PlayerCrouch(bool isCrouching)
    {
        anim.SetBool(CROUCH, isCrouching);
    }

    public void PlayerCrouchWalk(float magnitude)
    {
        anim.SetFloat(CROUCH_WALK, magnitude);
    }

    public void Shoot(bool isStanding)
    {
        if (isStanding)
        {
            anim.SetTrigger(STAND_SHOOT);
            networkAnim.SetTrigger(STAND_SHOOT);
        }
        else
        {
            anim.SetTrigger(CROUCH_SHOOT);
            networkAnim.SetTrigger(CROUCH_SHOOT);
        }

    }

    public void ReloadGun()
    {
        anim.SetTrigger(RELOAD);
        networkAnim.SetTrigger(RELOAD);
    }

    public void ChangeController(bool isPistol)
    {
        if (isPistol)
        {
            anim.runtimeAnimatorController = animController_Pistol;
        }
        else
        {
            anim.runtimeAnimatorController = animController_MachineGun;
        }

    }




}
