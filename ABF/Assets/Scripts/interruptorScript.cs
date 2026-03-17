using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [Header("Configurações da Luz")]
    [SerializeField] private Light targetLight;     
    [SerializeField] private Light ambientLight;    
    [SerializeField] private bool startOn = false;   

    [Header("Interação")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask switchLayer;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private string onPrompt = "Ligar Luz";
    [SerializeField] private string offPrompt = "Desligar Luz";
    [SerializeField] private float interactDistance = 2.5f;

    private bool isLookingAtSwitch = false;
    private bool isLightOn;
    private bool isSwitching = false;

    void Start()
    {
        isLightOn = startOn;

        if (targetLight != null)
            targetLight.enabled = isLightOn;

        if (ambientLight != null)
            ambientLight.enabled = !isLightOn; 
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
            {
                thisSwitchHit = true;
            }
        }

        if (thisSwitchHit)
        {
            if (!isLookingAtSwitch)
            {
                crosshairController.SetInteractable(isLightOn ? offPrompt : onPrompt);
                isLookingAtSwitch = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ToggleLight();
                crosshairController.SetInteractable(isLightOn ? offPrompt : onPrompt);
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

        isLightOn = !isLightOn;


        if (targetLight != null)
            targetLight.enabled = isLightOn;

        if (ambientLight != null)
            ambientLight.enabled = !isLightOn; 

        Debug.Log($"Luz principal: {(isLightOn ? "LIGADA" : "DESLIGADA")} | Luz ambiente: {(!isLightOn ? "LIGADA" : "DESLIGADA")}");

        isSwitching = false;
    }
}