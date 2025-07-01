using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SanityPostEffects : MonoBehaviour
{
    private Material effectMaterial;
    [SerializeField] private Shader effectShader;

    [Header("Vignette Settings")]
    [Range(0, 1)] public float vignetteIntensity = 0f;
    [Range(1, 10)] public float vignetteRoundness = 2f; 

    [Header("Desaturation")]
    [Range(0, 1)] public float desaturation = 0f;

    private void Awake()
    {
        effectShader = Shader.Find("Hidden/SanityEffects");
        effectMaterial = new Material(effectShader);
    }

    public void UpdateEffects(float insanityLevel, float maxVignette, float maxDesaturation)
    {
        vignetteIntensity = insanityLevel * maxVignette;
        desaturation = insanityLevel * maxDesaturation;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectMaterial != null)
        {
            effectMaterial.SetFloat("_VignetteIntensity", vignetteIntensity);
            effectMaterial.SetFloat("_VignetteRoundness", vignetteRoundness); 
            effectMaterial.SetFloat("_Desaturation", desaturation);
            Graphics.Blit(source, destination, effectMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}