using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public int SENSITIVITY = 2;
    private Vector3 rotation = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        

        rotation.x += Input.GetAxis("Mouse X") * SENSITIVITY;
        rotation.y += Input.GetAxis("Mouse Y") * SENSITIVITY;
        rotation.y = Mathf.Clamp(rotation.y, -80f, 80f);

        transform.localRotation = Quaternion.Euler(-rotation.y, rotation.x, 0);
    }
}
