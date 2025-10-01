using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VolumeSelector : MonoBehaviour
{
    public TMP_Text volumeText;
    public Button leftArrowButton; // Drag your left arrow button here in inspector
    public Button rightArrowButton; // Drag your right arrow button here in inspector
    public Button targetButton;
    public int volume = 5;
    public int minVolume = 0;
    public int maxVolume = 10;

    void Start()
    {
        UpdateVolumeText();
        AudioListener.volume = volume / 10f;

        // Add click listeners to buttons
        if (leftArrowButton != null)
            leftArrowButton.onClick.AddListener(DecreaseVolume);
        if (rightArrowButton != null)
            rightArrowButton.onClick.AddListener(IncreaseVolume);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == targetButton.gameObject)
        {
            if (Input.GetKeyDown(KeyCode.A))
                DecreaseVolume();

            if (Input.GetKeyDown(KeyCode.D))
                IncreaseVolume();
        }
    }

    public void DecreaseVolume()
    {
        if (volume > minVolume)
        {
            volume--;
            UpdateVolumeText();
            Debug.Log($"✅ Volume decreased to: {volume}");
        }
    }

    public void IncreaseVolume()
    {
        if (volume < maxVolume)
        {
            volume++;
            UpdateVolumeText();
            Debug.Log($"✅ Volume increased to: {volume}");
        }
    }

    void UpdateVolumeText()
    {
        volumeText.text = volume.ToString();
        AudioListener.volume = volume / 10f;
    }
}