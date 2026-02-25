using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeMultiplier = 0.03f; // Fine-tune amplitude (small for subtlety)
    [Range(0f, 10f)] public float shakeIntensity = 0f; // Controlled by SanitySystem

    private Vector3 originalLocalPos;

    private void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    private void LateUpdate() // Use LateUpdate to apply shake after other camera movements/rotations
    {
        if (shakeIntensity <= 0f)
        {
            transform.localPosition = originalLocalPos;
            return;
        }

        // Multi-octave Perlin for natural, jittery shake (2 octaves, higher freq for fast shake)
        float xShake = 0f;
        float freqX = 25f;
        float ampX = 1f;
        for (int i = 0; i < 2; i++)
        {
            xShake += (Mathf.PerlinNoise(Time.time * freqX, 42f) - 0.5f) * ampX;
            freqX *= 3f;
            ampX *= 0.5f;
        }

        float yShake = 0f;
        float freqY = 30f;
        float ampY = 1f;
        for (int i = 0; i < 2; i++)
        {
            yShake += (Mathf.PerlinNoise(69f, Time.time * freqY) - 0.5f) * ampY;
            freqY *= 2.7f;
            ampY *= 0.55f;
        }

        Vector3 shakeOffset = new Vector3(xShake, yShake, 0f) * shakeIntensity * shakeMultiplier;
        transform.localPosition = originalLocalPos + shakeOffset;
    }
}