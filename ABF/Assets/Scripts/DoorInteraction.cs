using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] LayerMask doorLayer;
    [SerializeField] CrosshairController crosshairController;
    [SerializeField] string openPrompt = "Open Door";
    [SerializeField] string closePrompt = "Close Door";

    Transform selectedDoor;
    private bool isLookingAtDoor = false;
    [SerializeField] private bool isDoorOpen = false;

    void Update()
    {
        RaycastHit hit;
        bool hitDoor = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 20, doorLayer);

        if (hitDoor && !isLookingAtDoor)
        {
            if (selectedDoor != null)
            {
                crosshairController.SetInteractable(isDoorOpen ? closePrompt : openPrompt);
            }
            else
            {
                crosshairController.SetInteractable(openPrompt);
            }
            isLookingAtDoor = true;
        }
        else if (!hitDoor && isLookingAtDoor)
        {
            crosshairController.SetNormal();
            isLookingAtDoor = false;
        }

        if (hitDoor && Input.GetMouseButtonDown(0))
        {
            selectedDoor = hit.collider.gameObject.transform;
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        if (selectedDoor == null) return;

        HingeJoint joint = selectedDoor.GetComponent<HingeJoint>();
        JointMotor motor = joint.motor;

        isDoorOpen = !isDoorOpen;

        float speedMultiplier = 60000;

        int leftDoor = selectedDoor.GetComponent<MeshRenderer>().localBounds.center.x > selectedDoor.localPosition.x ? 1 : -1;

        if (isDoorOpen)
        {
            
            motor.targetVelocity = speedMultiplier * Time.deltaTime * leftDoor;
            
        }
        if (!isDoorOpen)
        {
            
            motor.targetVelocity = -speedMultiplier * Time.deltaTime * leftDoor;
            
        }

        joint.motor = motor;

      
        if (isLookingAtDoor)
        {
            crosshairController.SetInteractable(isDoorOpen ? closePrompt : openPrompt);
        }
    }
}