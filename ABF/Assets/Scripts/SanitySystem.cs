using UnityEngine;
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
    [SerializeField] private float fadeOutDelay = 2f;
    [SerializeField] private float fadeOutDuration = 1f;

    private Vector3 originalCameraPos;
    public bool isInDarkness = false;
    private bool isLookingAtMonster = false;
    private float shakePower = 0f;
    private float timeAtFullSanity = 0f;
    private bool isFadingOut = false;
    private CanvasGroup sanityBarCanvasGroup;
    private SanityPostEffects sanityPostEffects;
    private float lastSanity = -1f;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
        originalCameraPos = playerCamera.transform.localPosition;

        if (GetComponent<LightSanitySystem>() == null)
            gameObject.AddComponent<LightSanitySystem>();

        if (sanityBarImage != null)
        {
            sanityBarCanvasGroup = sanityBarImage.GetComponent<CanvasGroup>() ?? sanityBarImage.gameObject.AddComponent<CanvasGroup>();
            UpdateSanityBar();
        }

        sanityPostEffects = playerCamera.GetComponent<SanityPostEffects>() ?? playerCamera.gameObject.AddComponent<SanityPostEffects>();
    }

    private void Update()
    {
        float prevSanity = currentSanity;

        if (isInDarkness)
            currentSanity -= darknessDrainRate * Time.deltaTime;
        else
            currentSanity += lightRecoveryRate * Time.deltaTime;

        if (isLookingAtMonster)
            currentSanity -= monsterViewDrainRate * Time.deltaTime;

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);

        if (Mathf.Abs(currentSanity - lastSanity) > 0.01f)
        {
            UpdateSanityBar();
            lastSanity = currentSanity;
        }

        UpdateVisualEffects();

        if (sanityBarImage != null && sanityBarCanvasGroup != null)
            HandleSanityBarVisibility();
    }

    private void UpdateSanityBar()
    {
        if (sanityBarImage != null)
            sanityBarImage.fillAmount = currentSanity / maxSanity;
    }

    private void HandleSanityBarVisibility()
    {
        float sanityPercentage = currentSanity / maxSanity;
        if (sanityPercentage >= 0.99f && !isFadingOut)
        {
            timeAtFullSanity += Time.deltaTime;
            if (timeAtFullSanity >= fadeOutDelay)
                StartFadeOut();
        }
        else if (sanityPercentage < 0.99f)
        {
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
            sanityBarCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
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
            sanityBarCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeOutDuration);
            yield return null;
        }
        sanityBarCanvasGroup.alpha = 1f;
        isFadingOut = false;
    }

    public void SetInDarkness(bool inDarkness) => isInDarkness = inDarkness;
    public void SetLookingAtMonster(bool lookingAtMonster) => isLookingAtMonster = lookingAtMonster;

    public void ApplyLoudNoiseEffect()
    {
        currentSanity = Mathf.Max(currentSanity - loudNoiseDrainAmount, 0f);
        UpdateSanityBar();
        FadeInBar();
    }

    public void ChangeSanity(float amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, 0f, maxSanity);
        UpdateSanityBar();
        FadeInBar();
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

        sanityPostEffects.UpdateEffects(inverseSanity, maxVignetteIntensity, maxDesaturation);
    }

    private void ApplyCameraShake()
    {
        if (shakePower > 0)
        {
            float xShake = (Mathf.PerlinNoise(Time.time * 30f, 0f) - 0.5f); // Increased frequency
            float yShake = (Mathf.PerlinNoise(0f, Time.time * 30f) - 0.5f);
            Vector3 shakeOffset = new Vector3(xShake, yShake, 0f) * shakePower * 2f; // Increased amplitude
            playerCamera.transform.localPosition = originalCameraPos + shakeOffset;
        }
        else
        {
            playerCamera.transform.localPosition = originalCameraPos;
        }
    }
}
