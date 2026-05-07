using System.Collections;
using UnityEngine;

public class PillsEffect : MonoBehaviour
{
    public float sanityRestoreAmount = 20;
    public int useTimes = 3;
    public AudioSource playerAudio;
    public AudioClip pillUseSound;

    private InventoryScript inventory;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryScript>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && inventory.GetCurrentItem() == gameObject)
            UsePills();
    }

    private void UsePills()
    {
        if (useTimes > 0)
        {
            var sanitySystem = FindObjectOfType<SanitySystem>();
            sanitySystem.ChangeSanity(sanityRestoreAmount);
            useTimes--;
            if (playerAudio != null && pillUseSound != null)
                playerAudio.PlayOneShot(pillUseSound);
        }

        if (useTimes <= 0)
            Destroy(gameObject);
    }
}