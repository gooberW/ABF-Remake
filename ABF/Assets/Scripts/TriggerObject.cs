using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject newObject;
    [SerializeField, Min(0.01f)] private float fadeDuration = 1f;
    [SerializeField] private bool destroyOnExit = true;
    [SerializeField] private AudioSource brotherSource;
    [SerializeField] private AudioClip disappearSound;

    private struct FadeTarget
    {
        public Renderer renderer;
        public int materialIndex;
        public Color originalColor;
    }

    private List<FadeTarget> targets;

    private void Awake()
    {
        if (triggerObject != null)
            triggerObject.SetActive(false);
        if (newObject != null)
            newObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerObject == null) return;
        if (newObject == null) return;

        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        PrepareTargets(); 
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(visible: true));
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerObject == null) return;
        if (newObject == null) return;

        if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;

        StopAllCoroutines();
        brotherSource.PlayOneShot(disappearSound);
        Destroy(triggerObject);
        newObject.SetActive(true);
    }

    private void PrepareTargets()
    {
        targets = new List<FadeTarget>();

        var renderers = triggerObject.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r == null) continue;
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                var mat = mats[i];
                Color orig = Color.white;

                if (mat != null && mat.HasProperty("_Color"))
                {
                    orig = mat.color;
                }
                else
                {
                    orig = new Color(1f, 1f, 1f, 1f);
                }

                var mpbInit = new MaterialPropertyBlock();
                var c0 = new Color(orig.r, orig.g, orig.b, 0f);
                mpbInit.SetColor("_Color", c0);
                r.SetPropertyBlock(mpbInit, i);

                targets.Add(new FadeTarget { renderer = r, materialIndex = i, originalColor = orig });
            }
        }
    }

    private IEnumerator FadeRoutine(bool visible)
    {
        if (visible)
            triggerObject.SetActive(true);

        if (targets == null || targets.Count == 0)
        {
            if (visible)
                triggerObject.SetActive(true);
            else
            {
                if (destroyOnExit) Destroy(triggerObject);
                else triggerObject.SetActive(false);
            }
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = visible ? t : 1f - t;

            foreach (var target in targets)
            {
                if (target.renderer == null) continue;

                var newColor = new Color(target.originalColor.r, target.originalColor.g, target.originalColor.b,
                                         target.originalColor.a * alpha);

                var mpb = new MaterialPropertyBlock();
                mpb.SetColor("_Color", newColor);
                target.renderer.SetPropertyBlock(mpb, target.materialIndex);
            }

            yield return null;
        }

        foreach (var target in targets)
        {
            if (target.renderer == null) continue;

            var mpb = new MaterialPropertyBlock();
            var finalAlpha = visible ? target.originalColor.a : 0f;
            var finalColor = new Color(target.originalColor.r, target.originalColor.g, target.originalColor.b, finalAlpha);
            mpb.SetColor("_Color", finalColor);
            target.renderer.SetPropertyBlock(mpb, target.materialIndex);
        }

        if (!visible)
        {
            if (destroyOnExit)
                Destroy(triggerObject);
            else
                triggerObject.SetActive(false);
        }
    }
}
