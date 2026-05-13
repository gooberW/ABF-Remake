using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Cinemachine;

public class SanitySystem : MonoBehaviour
{
    [Header("Sanity Settings")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float currentSanity = 100f;
    [SerializeField] private float darknessDrainRate = 3f;
    [SerializeField] private float lightRecoveryRate = 3.5f;
    [SerializeField] private float monsterViewDrainRate = 15f;
    [SerializeField] private float loudNoiseDrainAmount = 7f;

    [Header("Tick Settings (Optimization)")]
    [SerializeField] private float sanityTickInterval = 0.1f;

    [Header("Visual Effects")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxShakeIntensity = 1.5f;
    [SerializeField] private float maxVignetteIntensity = 2f;
    [SerializeField] private float maxDesaturation = 1f;

    [Header("UI Settings")]
    [SerializeField] private Image sanityBarImage;
    [SerializeField] private float fadeOutDelay = 2f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("Cutscene")]
    [SerializeField] private GameObject CutsceneTimeline;

    private SanityPostEffects sanityPostEffects;
    [SerializeField] private CameraShake cameraShake;

    private float lastSanity = -1f;
    private float sanityTickTimer = 0f;

    public bool isInDarkness = false;
    private bool isLookingAtMonster = false;

    private float timeAtFullSanity = 0f;
    private bool isFadingOut = false;

    private CanvasGroup sanityBarCanvasGroup;

    private bool isReloadingScene = false;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (GetComponent<LightSanitySystem>() == null)
            gameObject.AddComponent<LightSanitySystem>();

        if (sanityBarImage != null)
        {
            sanityBarCanvasGroup =
                sanityBarImage.GetComponent<CanvasGroup>() ??
                sanityBarImage.gameObject.AddComponent<CanvasGroup>();

            UpdateSanityBar();
        }

        sanityPostEffects =
            playerCamera.GetComponent<SanityPostEffects>() ??
            playerCamera.gameObject.AddComponent<SanityPostEffects>();

    }

    private void Update()
    {
        UpdateSanityTick();

        if (Mathf.Abs(currentSanity - lastSanity) > 0.01f)
        {
            UpdateSanityBar();
            lastSanity = currentSanity;
        }

        UpdateVisualEffects();

        if (sanityBarImage != null && sanityBarCanvasGroup != null)
            HandleSanityBarVisibility();
    }

    private void UpdateSanityTick()
    {
        if (isReloadingScene)
            return;

        sanityTickTimer += Time.deltaTime;

        if (sanityTickTimer >= sanityTickInterval)
        {
            float tickTime = sanityTickTimer;
            sanityTickTimer = 0f;

            float change = 0f;

            if (isInDarkness)
                change -= darknessDrainRate * tickTime;
            else
                change += lightRecoveryRate * tickTime;

            if (isLookingAtMonster)
                change -= monsterViewDrainRate * tickTime;

            currentSanity = Mathf.Clamp(currentSanity + change, 0f, maxSanity);

  
            if (currentSanity <= 0f)
            {
                ReloadCurrentScene();
            }
        }
    }

    private void ReloadCurrentScene()
    {
        if (isReloadingScene)
            return;

        isReloadingScene = true;

        if (CutsceneTimeline != null)
        {
            CutsceneTimeline.SetActive(false);
        }

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
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

    private IEnumerator FadeOutBar()
    {
        float elapsedTime = 0f;
        float startAlpha = sanityBarCanvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            sanityBarCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);

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

    private IEnumerator FadeInBarCoroutine()
    {
        float elapsedTime = 0f;
        float startAlpha = sanityBarCanvasGroup.alpha;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            sanityBarCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeOutDuration);

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
        currentSanity = Mathf.Max(currentSanity - loudNoiseDrainAmount, 0f);

        UpdateSanityBar();
        FadeInBar();
    }

    public void ChangeSanity(float amount)
    {
        currentSanity =
            Mathf.Clamp(currentSanity + amount, 0f, maxSanity);

        UpdateSanityBar();
        FadeInBar();
    }

    private void UpdateVisualEffects()
    {
        float sanityPercentage = currentSanity / maxSanity;
        float inverseSanity = 1f - sanityPercentage;

        float shakeFactor =
            Mathf.Clamp01((inverseSanity - 0.45f) / 0.55f);

        float shakePower = shakeFactor * maxShakeIntensity;

        if (cameraShake != null)
            cameraShake.shakeIntensity = shakePower;

        sanityPostEffects.UpdateEffects(
            inverseSanity,
            maxVignetteIntensity,
            maxDesaturation
        );
    }
}