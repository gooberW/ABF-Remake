using UnityEngine;
using UnityEngine.Rendering;

public class CameraMotion : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _step;
    [SerializeField] private PlayerScript _player;
    private Animator _animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputMagnitude = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

        float speed = 0f;
        if (inputMagnitude > 0.1f)
        {
            if (_player.IsGrounded())
            {
                speed = _player.playerSpeed;
            }

        }else
        {
            speed = 0;
        }

            _animator.SetFloat("PlayerSpeed", speed, 0.2f, Time.deltaTime);
    }

    void PlayFootstep()
    {
        _source.pitch = Random.Range(0.8f, 1.2f);
        if (_player.playerSpeed == _player.RUN_SPEED)
        {
            _source.volume = 0.8f;
        }
        else
        {
            _source.volume = 0.5f;
        }
        _source.PlayOneShot(_step);
    }
}
