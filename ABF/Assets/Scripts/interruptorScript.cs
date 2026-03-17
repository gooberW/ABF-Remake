using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Configurações da Luz")]
    [SerializeField] private Light targetLight;
    [SerializeField] private Light ambientLight;

    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask switchLayer;
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
        RaycastHit hit;
        bool hitSwitch = Physics.Raycast(
            cam.transform.position,
            cam.transform.forward,
            out hit,
            interactDistance,
            switchLayer
        );

        bool thisSwitchHit = false;

        if (hitSwitch && hit.collider != null)
        {
            Transform hitTransform = hit.collider.transform;
            if (hitTransform == this.transform || hitTransform.IsChildOf(this.transform))
                thisSwitchHit = true;
        }

        if (thisSwitchHit)
        {
            if (!isLookingAtSwitch)
            {
                crosshairController.SetInteractable(targetLight.enabled ? offPrompt : onPrompt);
                isLookingAtSwitch = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ToggleLight();
                crosshairController.SetInteractable(targetLight.enabled ? offPrompt : onPrompt);
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
            manager.ToggleLight(targetLight);

        isSwitching = false;
    }


    private void OnLightChanged(Light changedLight, bool isOn)
    {
        if (changedLight != targetLight || ambientLight == null) return;

        ambientLight.enabled = !isOn;
    }
}