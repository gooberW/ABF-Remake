using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VolumeSelector : MonoBehaviour
{
    public TMP_Text volumeText; 
    public int volume = 5;  
    public int minVolume = 0;
    public int maxVolume = 10;

    void Start()
    {
        UpdateVolumeText();
        AudioListener.volume = volume / 10f;
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == volumeText.gameObject)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                volume = Mathf.Max(minVolume, volume - 1);
                UpdateVolumeText();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                volume = Mathf.Min(maxVolume, volume + 1);
                UpdateVolumeText();
            }
        }

    }

    void UpdateVolumeText()
    {
        volumeText.text = volume.ToString();
        AudioListener.volume = volume / 10f; 
    }
}
