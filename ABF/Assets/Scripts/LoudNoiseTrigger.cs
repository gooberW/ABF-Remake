using UnityEngine;

public class LoudNoiseTrigger : MonoBehaviour
{
    [SerializeField] private SanitySystem sanitySystem;
    [SerializeField] private float loudNoiseThreshold = 0.8f; // Volume threshold

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // Simple loudness check (better: use RMS calculation)
        for (int i = 0; i < data.Length; i++)
        {
            if (Mathf.Abs(data[i]) > loudNoiseThreshold)
            {
                sanitySystem.ApplyLoudNoiseEffect();
                break;
            }
        }
    }
}