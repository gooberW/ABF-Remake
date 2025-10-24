using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Idoorincteractable
{
    
    private HingeJoint joint;
    private Transform T;
    private bool isDoorOpen;

    public bool IsDoorOpen { get => isDoorOpen; set => isDoorOpen = value; }

    private void Start()
    {
        joint = GetComponent<HingeJoint>();
    }

    public void OnDoorInteratable()
    {
        HingeJoint joint = T.GetComponent<HingeJoint>();
        JointMotor motor = joint.motor;

        isDoorOpen = !isDoorOpen;

        float speedMultiplier = 60000;

        int leftDoor = T.GetComponent<MeshRenderer>().localBounds.center.x > T.localPosition.x ? 1 : -1;

        if (isDoorOpen)
        {

            motor.targetVelocity = speedMultiplier * Time.deltaTime * leftDoor;

        }
        if (!isDoorOpen)
        {

            motor.targetVelocity = -speedMultiplier * Time.deltaTime * leftDoor;

        }

        joint.motor = motor;


        //if (isLookingAtDoor)
        //{
        //    crosshairController.SetInteractable(isDoorOpen ? closePrompt : openPrompt);
        //}
    }
}
