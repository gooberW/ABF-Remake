using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Parameters")]
    public float WALK_SPEED = 1.5f;
    public float RUN_SPEED = 4f;
    public float CROUCH_SPEED = 1f;
    [SerializeField] private static float MAX_STAMINA = 100f;
    [SerializeField] private float _staminaRegenRate = 10f;
    [SerializeField] private LayerMask ceilingLayerMask;
    //-----------

    [SerializeField] private float _currentHealth;
    private float _currentStamina;
    private Rigidbody _rb;
    private CapsuleCollider _capsule;
    [SerializeField] private GameObject _holder;
    [SerializeField] private Camera _cam;

    //private static float JUMP_FORCE = 5f;
    private float horizontalInput;
    private float verticalInput;
    private bool isCrouching = false;

    [HideInInspector] public float playerSpeed;

    // Toggle used by other systems (DialogueManager) to enable/disable movement
    public static bool CanMove = true;

    private float originalHeight;
    private Vector3 originalCenter;
    private Vector3 originalHolderPos;

    private float crouchHeight;
    private Vector3 crouchCenter;
    private Vector3 crouchHolderPos;

    private bool hasCeiling;
    [Header("References")]
    //[SerializeField] private NotebookScript _notebook;
    [SerializeField] private CanvasGroup _staminaBar;
    [SerializeField] private UnityEngine.UI.Image _slider;

    private bool isLookingAtDoor = false;
    

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

        _currentStamina = MAX_STAMINA;
    }


    void FixedUpdate()
    {
        // If movement is disabled, keep vertical velocity and zero horizontal movement
        if (!CanMove)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            return;
        }

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
        // If movement is disabled, skip movement-related updates (crouch/run/stamina)
        if (!CanMove) return;

        //hasCeiling = CheckCeiling();
        //Debug.Log(hasCeiling);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                isCrouching = true;
            }
        }
        else
        {
            
            if (isCrouching && !CheckCeiling())
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
        CheckStamina();
        //Debug.Log(_currentStamina);

        
    }


    void CheckStamina()
    {
        if (_currentStamina < MAX_STAMINA && playerSpeed <= WALK_SPEED + 0.1f)
        {
            _currentStamina += _staminaRegenRate * Time.deltaTime;
        }
        else if (_currentStamina > MAX_STAMINA)
        {
            _currentStamina = MAX_STAMINA;
        }
        else if (_currentStamina <= 0)
        {
            playerSpeed = WALK_SPEED;
        }
        else if ((_currentStamina > 0 && horizontalInput > WALK_SPEED + 0.1f) || verticalInput > WALK_SPEED + 0.11f)
        {
            _currentStamina -= _staminaRegenRate * 1.3f * Time.deltaTime;
        }


        if (_staminaBar != null && _slider != null)
        {
            _slider.fillAmount = _currentStamina / MAX_STAMINA;

            if (_currentStamina >= MAX_STAMINA)
            {
                _staminaBar.alpha = Mathf.Lerp(_staminaBar.alpha, 0, Time.deltaTime * 4f);
            }
            else
            {
                _staminaBar.alpha = Mathf.Lerp(_staminaBar.alpha, 1, Time.deltaTime * 2f);
            }
        }

    }


    public bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position + Vector3.up * 0.85f, Vector3.down, out hit, 1.1f);
    }

    /**
     * Esta func vai projetar um capsule para ver se tem algum objeto a 
     * bloquear a parte de cima do player. Assim quando o player estiver crouched, 
     * ele fica até ter um espaço vazio acima da "cabeça".
     */
    public bool CheckCeiling()
    {
        RaycastHit hit;

        float radius = _capsule.radius * 0.95f;
        float height = _capsule.height;
        Vector3 center = transform.position + _capsule.center;

        // We'll cast upward from the top of the capsule
        Vector3 origin = center + Vector3.up * (height / 2f - radius);
        Vector3 direction = Vector3.up;
        float checkDistance = 0.2f;

        bool hitSomething = Physics.SphereCast(origin, radius, direction, out hit, checkDistance, ceilingLayerMask, QueryTriggerInteraction.Ignore);

        Debug.DrawLine(origin, origin + direction * checkDistance, hitSomething ? Color.red : Color.green);

        if (hitSomething)
        {
            Debug.Log("Ceiling hit: " + hit.collider.gameObject.name);
        }

        return hitSomething;
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