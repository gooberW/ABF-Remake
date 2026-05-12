using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSystem : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] concreteFootsteps;
    [SerializeField] private AudioClip[] sandFootsteps;
    [SerializeField] private AudioClip[] grassFootsteps;

    [Header("Step Timing")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;
    [SerializeField] private float crouchStepInterval = 0.7f;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float walkVolume = 0.6f;
    [Range(0f, 1f)][SerializeField] private float runVolume = 0.9f;
    [Range(0f, 1f)][SerializeField] private float crouchVolume = 0.3f;

    [Header("Ground Detection")]
    [Tooltip("Tick Ground, Obstacle, Sand and Grass — every layer the player can walk on. Do NOT include the Player layer.")]
    [SerializeField] private LayerMask groundLayerMask;

    private const string LAYER_GROUND = "Ground";
    private const string LAYER_OBSTACLE = "Obstacle";
    private const string LAYER_SAND = "Sand";
    private const string LAYER_GRASS = "Grass";

    private AudioSource _audioSource;
    private PlayerScript _player;
    private Rigidbody _rb;
    private float _stepTimer;
    private int _lastClipIndex = -1;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _player = GetComponent<PlayerScript>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!PlayerScript.CanMove)
        {
            Debug.Log("[Footstep] CanMove is false");
            return;
        }

        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        Debug.Log($"[Footstep] Horizontal velocity magnitude: {horizontalVelocity.magnitude}");

        bool isMoving = horizontalVelocity.magnitude > 0.1f;
        if (!isMoving)
        {
            Debug.Log("[Footstep] Not moving");
            return;
        }

        if (!IsGrounded(out string groundLayer))
        {
            Debug.Log("[Footstep] IsGrounded returned false — raycast missed, check groundLayerMask in Inspector");
            return;
        }

        float interval = GetStepInterval();
        _stepTimer += Time.deltaTime;

        if (_stepTimer >= interval)
        {
            _stepTimer = 0f;
            PlayFootstep(groundLayer);
        }
    }

    private bool IsGrounded(out string layerName)
    {
        layerName = string.Empty;

        Vector3 origin = transform.position + Vector3.up * 0.85f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 5f, groundLayerMask, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform == transform)
                continue;

            layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            Debug.Log($"[Footstep] Standing on layer: '{layerName}' ({hit.collider.gameObject.name})");
            return true;
        }

        return false;
    }

    private void PlayFootstep(string layerName)
    {
        AudioClip[] clips = GetClipsForLayer(layerName);
        if (clips == null || clips.Length == 0) return;

        int index;
        if (clips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do { index = Random.Range(0, clips.Length); }
            while (index == _lastClipIndex);
        }

        _lastClipIndex = index;
        _audioSource.PlayOneShot(clips[index], GetVolume());
    }

    private AudioClip[] GetClipsForLayer(string layerName)
    {
        switch (layerName)
        {
            case LAYER_GROUND:
            case LAYER_OBSTACLE:
                return concreteFootsteps;

            case LAYER_SAND:
                return sandFootsteps;

            case LAYER_GRASS:
                return grassFootsteps;

            default:
                return concreteFootsteps;
        }
    }

    private float GetStepInterval()
    {
        if (_player.playerSpeed >= _player.RUN_SPEED - 0.2f)
            return runStepInterval;

        if (_player.playerSpeed <= _player.CROUCH_SPEED + 0.1f)
            return crouchStepInterval;

        return walkStepInterval;
    }

    private float GetVolume()
    {
        if (_player.playerSpeed >= _player.RUN_SPEED - 0.2f)
            return runVolume;

        if (_player.playerSpeed <= _player.CROUCH_SPEED + 0.1f)
            return crouchVolume;

        return walkVolume;
    }
}