using UnityEngine;
public class LightSwitch : MonoBehaviour
{
    [Header("Configurações da Luz")]
    [SerializeField] private Light targetLight;
    [SerializeField] private Light ambientLight;

    [Header("Som")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchOnSound;
    [SerializeField] private AudioClip switchOffSound;

    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask switchLayer;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private string onPrompt = "Ligar Luz";
    [SerializeField] private string offPrompt = "Desligar Luz";
    [SerializeField] private float interactDistance = 2.5f;
    private bool isLookingAtSwitch = false;
    private bool isSwitching = false;
    private LightManager manager;
    void Start()
    {
        if (targetLight != null)
            targetLight.enabled = false;
        if (ambientLight != null)
            ambientLight.enabled = true;
        manager = FindObjectOfType<LightManager>();
        if (manager != null)
            manager.OnLightChanged += OnLightChanged;
    }
    void OnDestroy()
    {
        if (manager != null)
            manager.OnLightChanged -= OnLightChanged;
    }
    void Update()
    {
        if (cam == null || crosshairController == null) return;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        bool visible = false;
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                if ((switchLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    visible = true;
                    break;
                }
                continue;
            }
            if ((obstacleMask.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                visible = false;
                break;
            }
        }
        if (visible)
        {
            if (!isLookingAtSwitch)
            {
                crosshairController.SetInteractable(targetLight != null && targetLight.enabled ? offPrompt : onPrompt);
                isLookingAtSwitch = true;
            }
            if (Input.GetMouseButtonDown(0))
            {
                ToggleLight();
                crosshairController.SetInteractable(targetLight != null && targetLight.enabled ? offPrompt : onPrompt);
            }
        }
        else
        {
            if (isLookingAtSwitch)
            {
                crosshairController.SetNormal();
                isLookingAtSwitch = false;
            }
        }
    }
    private void ToggleLight()
    {
        if (isSwitching) return;
        isSwitching = true;
        if (manager != null && targetLight != null)
        {
            bool willTurnOn = !targetLight.enabled;
            manager.ToggleLight(targetLight);
            PlaySwitchSound(willTurnOn);
        }
        isSwitching = false;
    }
    private void PlaySwitchSound(bool turningOn)
    {
        if (audioSource == null) return;
        AudioClip clip = turningOn ? switchOnSound : switchOffSound;
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
    private void OnLightChanged(Light changedLight, bool isOn)
    {
        if (changedLight != targetLight || ambientLight == null) return;
        ambientLight.enabled = !isOn;
    }
}