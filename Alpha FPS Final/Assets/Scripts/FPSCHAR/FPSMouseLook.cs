using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouseLook : MonoBehaviour {
    //this is the second video from AwesomeTuts on Udemy
    //Doing the ability to look with a mouse

    public enum RotationAxes { MouseX, MouseY };
    public RotationAxes axes = RotationAxes.MouseY;

    private float currentSensitivity_X = 0.5f;
    private float currentSensitivity_Y = 0.5f;

    private float sensitivity_X = 1.5f;
    private float sensitivity_Y = 1.5f;

    private float Rotation_X, Rotation_Y;

    private float minimum_X = -360f;
    private float maximum_X = 360f;

    private float minimum_Y = -60f;
    private float maximum_Y = 60f;

    private Quaternion originalRotation;

    private float mouseSensitivity = 1.7f;

	// Use this for initialization
	void Start () {
        //without this line of code in the start the player will not 
        //be able to rotate at all. Move but not rotate
        originalRotation = transform.rotation;

	}
	
    

	// Update is called once per frame
	void LateUpdate () {
        HandleRotation();
	}

    // the code down below is what keeps the camera from going crazy in all directions.
    //just to make sure that i don't see my feet or anything extra 
    float ClampThoseAngles (float min, float max, float angle)
    {
        if (angle < 360f)
        {
            angle += 360f;
        }

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(min, max, angle);
    }

    //this function below is what handles all of the rotation on both of the up/down and the left/right 
    //on the mouse this function 
    void HandleRotation()
    {
        if (currentSensitivity_X != mouseSensitivity || currentSensitivity_Y != mouseSensitivity )
        {
            currentSensitivity_X = currentSensitivity_Y = mouseSensitivity;
   
        }

        sensitivity_X = currentSensitivity_X;
        sensitivity_Y = currentSensitivity_Y;

        if (axes == RotationAxes.MouseX)
        {
            Rotation_X += Input.GetAxis("Mouse X") * sensitivity_X;

            Rotation_X = ClampThoseAngles(Rotation_X, minimum_X, maximum_X);
            Quaternion xQuaterion = Quaternion.AngleAxis(Rotation_X, Vector3.up);
            transform.localRotation = originalRotation * xQuaterion;
        }

        if (axes == RotationAxes.MouseY)
        {
            Rotation_Y += Input.GetAxis("Mouse Y") * sensitivity_Y;

            Rotation_Y = ClampThoseAngles(Rotation_Y, minimum_Y, maximum_Y);

            Quaternion yQuaternion = Quaternion.AngleAxis(-Rotation_Y, Vector3.right);

            transform.localRotation = originalRotation * yQuaternion;

        }

    }



}//class



























































