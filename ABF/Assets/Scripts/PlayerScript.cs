using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
// cor da fog 29081E
public class PlayerScript : MonoBehaviour
{
    

    [SerializeField] private float _currentHealth;
    private float _currentStamina;
    private Rigidbody _rb;
    [SerializeField] private Camera _cam;
    [SerializeField] private float WALK_SPEED = 4f;
    [SerializeField] private float RUN_SPEED = 10f;
    //private static float JUMP_FORCE = 5f;
    private float horizontalInput;
    private float verticalInput;

    [HideInInspector] public float playerSpeed;



    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal") * playerSpeed;
        verticalInput = Input.GetAxis("Vertical") * playerSpeed;


        Vector3 camForward = _cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 moveDirection = (camForward * verticalInput) + (_cam.transform.right * horizontalInput);

        if (IsGrounded())
        {

            _rb.velocity = new Vector3(moveDirection.x, _rb.velocity.y, moveDirection.z);
        }

    }

    void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            playerSpeed = Mathf.Lerp(playerSpeed, RUN_SPEED, Time.deltaTime * 8f);
        }
        else //if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            playerSpeed = Mathf.Lerp(playerSpeed, WALK_SPEED, Time.deltaTime * 8f);
        }
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position + Vector3.up * 0.85f, Vector3.down, out hit, 1.1f);
    }

}