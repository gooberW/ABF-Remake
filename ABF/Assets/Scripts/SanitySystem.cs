using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 100f;
    [SerializeField] private float darknessDrainRate = 5f;
    [SerializeField] private float lightRecoveryRate = 3f;
    [SerializeField] private float monsterViewDrainRate = 10f;
    [SerializeField] private float loudNoiseDrainAmount = 15f;

    [Header("Visual Effects")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxShakeIntensity = 0.5f;
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    [SerializeField] private float maxDesaturation = 0.8f;

    [Header("UI Settings")]
    [SerializeField] private Image sanityBarImage;
    [SerializeField] private float fadeOutDelay = 2f; // Time to wait before fading out
    [SerializeField] private float fadeOutDuration = 1f; // How long the fade out takes

    private Vector3 originalCameraPos;
    public bool isInDarkness = false;
    private bool isLookingAtMonster = false;
    private float shakePower = 0f;
    private float timeAtFullSanity = 0f;
    private bool isFadingOut = false;
    private CanvasGroup sanityBarCanvasGroup;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        originalCameraPos = playerCamera.transform.localPosition;

        if (GetComponent<LightSanitySystem>() == null)
        {
            gameObject.AddComponent<LightSanitySystem>();
        }

        // Initialize sanity bar if it exists
        if (sanityBarImage != null)
        {
            // Get or add CanvasGroup for fading
            sanityBarCanvasGroup = sanityBarImage.GetComponent<CanvasGroup>();
            if (sanityBarCanvasGroup == null)
            {
                sanityBarCanvasGroup = sanityBarImage.gameObject.AddComponent<CanvasGroup>();
            }
            UpdateSanityBar();
        }
    }

    private void Update()
    {
        if (isInDarkness)
        {
            currentSanity -= darknessDrainRate * Time.deltaTime;
        }
        else
        {
            currentSanity += lightRecoveryRate * Time.deltaTime;
        }

        if (isLookingAtMonster)
        {
            currentSanity -= monsterViewDrainRate * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);
        UpdateVisualEffects();

        // Update the sanity bar UI
        if (sanityBarImage != null && sanityBarCanvasGroup != null)
        {
            UpdateSanityBar();
            HandleSanityBarVisibility();
        }
    }

    private void UpdateSanityBar()
    {
        float sanityPercentage = currentSanity / maxSanity;
        sanityBarImage.fillAmount = sanityPercentage;
    }

    private void HandleSanityBarVisibility()
    {
        float sanityPercentage = currentSanity / maxSanity;

        // If sanity is full and not already fading out
        if (sanityPercentage >= 0.99f && !isFadingOut)
        {
            timeAtFullSanity += Time.deltaTime;

            // After delay, start fading out
            if (timeAtFullSanity >= fadeOutDelay)
            {
                StartFadeOut();
            }
        }
        else if (sanityPercentage < 0.99f)
        {
            // Sanity dropped below full, reset timer and make sure bar is visible
            timeAtFullSanity = 0f;
            if (isFadingOut || sanityBarCanvasGroup.alpha < 1f)
            {
                StopAllCoroutines();
                FadeInBar();
            }
        }
    }

    private void StartFadeOut()
    {
        isFadingOut = true;
        StopAllCoroutines();
        StartCoroutine(FadeOutBar());
    }

    private System.Collections.IEnumerator FadeOutBar()
    {
        float elapsedTime = 0f;
        float startAlpha = sanityBarCanvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            sanityBarCanvasGroup.alpha = newAlpha;
            yield return null;
        }

        sanityBarCanvasGroup.alpha = 0f;
        isFadingOut = false;
    }

    private void FadeInBar()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInBarCoroutine());
    }

    private System.Collections.IEnumerator FadeInBarCoroutine()
    {
        float elapsedTime = 0f;
        float startAlpha = sanityBarCanvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeOutDuration);
            sanityBarCanvasGroup.alpha = newAlpha;
            yield return null;
        }

        sanityBarCanvasGroup.alpha = 1f;
        isFadingOut = false;
    }

    public void SetInDarkness(bool inDarkness)
    {
        isInDarkness = inDarkness;
    }

    public void SetLookingAtMonster(bool lookingAtMonster)
    {
        isLookingAtMonster = lookingAtMonster;
    }

    public void ApplyLoudNoiseEffect()
    {
        currentSanity -= loudNoiseDrainAmount;
        currentSanity = Mathf.Max(currentSanity, 0f);

        // Update UI immediately and ensure bar is visible
        if (sanityBarImage != null && sanityBarCanvasGroup != null)
        {
            UpdateSanityBar();
            FadeInBar();
        }
    }

    public void ChangeSanity(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        // Update UI immediately and ensure bar is visible
        if (sanityBarImage != null && sanityBarCanvasGroup != null)
        {
            UpdateSanityBar();
            FadeInBar();
        }
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