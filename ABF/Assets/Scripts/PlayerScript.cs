using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
// cor da fog 29081E
public class PlayerScript : MonoBehaviour
{
    

    [SerializeField] private float _currentHealth;
    private float _currentStamina;
    private Rigidbody _rb;
    private CapsuleCollider _capsule;
    [SerializeField] private GameObject _holder;
    [SerializeField] private Camera _cam;
    [SerializeField] public static float WALK_SPEED = 1.5f;
    [SerializeField] public static float RUN_SPEED = 4f;
    [SerializeField] public static float CROUCH_SPEED = 1f;
    //private static float JUMP_FORCE = 5f;
    private float horizontalInput;
    private float verticalInput;
    private bool isCrouching = false;

    [HideInInspector] public float playerSpeed;


    private float originalHeight;
    private Vector3 originalCenter;
    private Vector3 originalHolderPos;

    private float crouchHeight;
    private Vector3 crouchCenter;
    private Vector3 crouchHolderPos;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();

        originalHeight = _capsule.height;
        originalCenter = _capsule.center;
        originalHolderPos = _holder.transform.localPosition;

        crouchHeight = originalHeight / 2f;
        crouchCenter = originalCenter - new Vector3(0, (originalHeight - crouchHeight) / 2f, 0);
        crouchHolderPos = originalHolderPos + new Vector3(0, -0.5f, 0);
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
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                isCrouching = true;
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
            }
        }

        if (isCrouching)
        {
            playerSpeed = Mathf.Lerp(playerSpeed, CROUCH_SPEED, Time.deltaTime * 8f);
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            playerSpeed = Mathf.Lerp(playerSpeed, RUN_SPEED, Time.deltaTime * 8f);
        }
        else
        {
            playerSpeed = Mathf.Lerp(playerSpeed, WALK_SPEED, Time.deltaTime * 8f);
        }

        Crouch();
    }


    public bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position + Vector3.up * 0.85f, Vector3.down, out hit, 1.1f);
    }


    private void Crouch()
    {
        if (isCrouching)
        {
            _capsule.height = Mathf.Lerp(_capsule.height, crouchHeight, Time.deltaTime * 6f);
            _capsule.center = Vector3.Lerp(_capsule.center, crouchCenter, Time.deltaTime * 6f);
            _holder.transform.localPosition = Vector3.Lerp(_holder.transform.localPosition, crouchHolderPos, Time.deltaTime * 6f);
        }
        else
        {
            _capsule.height = Mathf.Lerp(_capsule.height, originalHeight, Time.deltaTime * 6f);
            _capsule.center = Vector3.Lerp(_capsule.center, originalCenter, Time.deltaTime * 6f);
            _holder.transform.localPosition = Vector3.Lerp(_holder.transform.localPosition, originalHolderPos, Time.deltaTime * 6f);
        }
    }


}