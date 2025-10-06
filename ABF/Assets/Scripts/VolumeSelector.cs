using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Audio;

public class VolumeSelector : MonoBehaviour
{
    public TMP_Text volumeText;
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button targetButton;

    [Range(0, 10)] public int volume = 5;
    public int minVolume = 0;
    public int maxVolume = 10;

    // Mixer
    public AudioMixer audioMixer;
    public string exposedParameterName; 

    // Sounds
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        UpdateVolumeText();
        SetMixerVolume(volume);

        if (leftArrowButton != null)
        {
            leftArrowButton.onClick.AddListener(DecreaseVolume);
            SetupButtonSounds(leftArrowButton);
        }
        if (rightArrowButton != null)
        {
            rightArrowButton.onClick.AddListener(IncreaseVolume);
            SetupButtonSounds(rightArrowButton);
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == targetButton.gameObject)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                DecreaseVolume();
                PlayClickSound();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                IncreaseVolume();
                PlayClickSound();
            }
        }
    }

    public void DecreaseVolume()
    {
        if (volume > minVolume)
        {
            volume--;
            UpdateVolumeText();
            SetMixerVolume(volume);
        }
    }

    public void IncreaseVolume()
    {
        if (volume < maxVolume)
        {
            volume++;
            UpdateVolumeText();
            SetMixerVolume(volume);
        }
    }

    void UpdateVolumeText()
    {
        volumeText.text = volume.ToString();
    }

    void SetMixerVolume(int vol)
    {
        // Convert 0-10 to decibels (linear → log scale)
        float dB = Mathf.Lerp(-80f, 0f, vol / 10f);
        audioMixer.SetFloat(exposedParameterName, dB);
    }

    private void SetupButtonSounds(Button button)
    {
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnterEntry.callback.AddListener((data) => { OnButtonHover(); });
        eventTrigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        pointerClickEntry.callback.AddListener((data) => { OnButtonClick(); });
        eventTrigger.triggers.Add(pointerClickEntry);
    }

    private void OnButtonHover() => PlayHoverSound();
    private void OnButtonClick() => PlayClickSound();

    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}
