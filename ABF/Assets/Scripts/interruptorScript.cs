using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Configurações da Luz")]
    [SerializeField] private Light targetLight;
    [SerializeField] private Light ambientLight;

    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask switchLayer;
    [SerializeField] private LayerMask obstacleMask; // new: layers that block line of sight (e.g. "Obstacle")
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

            // If we hit this switch (or a child) first -> visible
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                // ensure the hit actually belongs to switchLayer (prevents non-switch colliders on same transform)
                if ((switchLayer.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    visible = true;
                    break;
                }
                // if not on switchLayer, continue scanning
                continue;
            }

            // If the hit is on an obstacle layer, it blocks view -> not visible
            if ((obstacleMask.value & (1 << hit.collider.gameObject.layer)) != 0)
            {
                visible = false;
                break;
            }

            // Otherwise, ignore and continue scanning (e.g., triggers, decorative colliders)
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
            manager.ToggleLight(targetLight);

        isSwitching = false;
    }

    private void OnLightChanged(Light changedLight, bool isOn)
    {
        if (changedLight != targetLight || ambientLight == null) return;

        ambientLight.enabled = !isOn;
    }
}