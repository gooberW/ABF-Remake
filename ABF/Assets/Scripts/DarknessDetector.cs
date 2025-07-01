using UnityEngine;

public class DarknessDetector : MonoBehaviour
{
    [SerializeField] private SanitySystem sanitySystem;
    [SerializeField] private float checkRadius = 5f; 
    [SerializeField] private LayerMask lightLayer;
    private void Update()
    {
        bool isInLight = CheckForNearbyLights();
        sanitySystem.SetInDarkness(!isInLight); 
    }

    private bool CheckForNearbyLights()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, lightLayer);

        foreach (var collider in hitColliders)
        {
            Light light = collider.GetComponent<Light>();
            if (light != null && light.enabled && light.intensity > 0.1f)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}