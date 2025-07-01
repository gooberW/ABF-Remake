using UnityEngine;
using UnityEngine.Rendering;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 100f;
    [SerializeField] private float darknessDrainRate = 5f; 
    [SerializeField] private float monsterViewDrainRate = 10f; 
    [SerializeField] private float loudNoiseDrainAmount = 15f; 

    [Header("Visual Effects")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxShakeIntensity = 0.5f;
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    [SerializeField] private float maxDesaturation = 0.8f;

    private Vector3 originalCameraPos;
    private bool isInDarkness = false;
    private bool isLookingAtMonster = false;
    private float shakePower = 0f;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        originalCameraPos = playerCamera.transform.localPosition;
    }

    private void Update()
    {
        if (isInDarkness)
            currentSanity -= darknessDrainRate * Time.deltaTime;

        if (isLookingAtMonster)
            currentSanity -= monsterViewDrainRate * Time.deltaTime;

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        Debug.Log($"Current Sanity -> {currentSanity}");

        UpdateVisualEffects();
    }

    public void SetInDarkness(bool inDarkness)
    {
        isInDarkness = inDarkness;
        Debug.Log("Not on light source");
    }

    public void SetLookingAtMonster(bool lookingAtMonster)
    {
        isLookingAtMonster = lookingAtMonster;
    }

    public void ApplyLoudNoiseEffect()
    {
        currentSanity -= loudNoiseDrainAmount;
    }

    private void UpdateVisualEffects()
    {
        float sanityPercentage = currentSanity / maxSanity;
        float inverseSanity = 1f - sanityPercentage;

        if (inverseSanity > 0.1f)
        {
            shakePower = inverseSanity * maxShakeIntensity;
            ApplyCameraShake();
        }
        else
        {
            playerCamera.transform.localPosition = originalCameraPos;
        }

        if (playerCamera.GetComponent<SanityPostEffects>() == null)
        {
            playerCamera.gameObject.AddComponent<SanityPostEffects>();
        }
        playerCamera.GetComponent<SanityPostEffects>().UpdateEffects(inverseSanity, maxVignetteIntensity, maxDesaturation);
    }

    private void ApplyCameraShake()
    {
        if (shakePower > 0)
        {

            float xShake = (Mathf.PerlinNoise(Time.time * 3f, 0f) - 0.5f);
            float yShake = (Mathf.PerlinNoise(0f, Time.time * 3f) - 0.5f);

            Vector3 shakeOffset = new Vector3(xShake, yShake, 0f) * shakePower;
            playerCamera.transform.localPosition = originalCameraPos + shakeOffset;
        }
        else
        {
            playerCamera.transform.localPosition = originalCameraPos;
        }
    }
}