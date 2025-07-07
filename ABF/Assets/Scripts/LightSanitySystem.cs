using UnityEngine;
using System.Linq;

public class LightSanitySystem : MonoBehaviour
{
    [Header("Settings")]
    public float checkInterval = 0.25f;
    public float rangeMultiplier = 1.5f;
    public LayerMask obstructionLayers;

    private SanitySystem sanitySystem;
    private float checkTimer;
    private Collider playerCollider;
    private Light[] allLights;

    private void Awake()
    {
        sanitySystem = GetComponent<SanitySystem>();
        if (sanitySystem == null)
        {
            Debug.LogError("SanitySystem component not found!");
            enabled = false;
            return;
        }

        playerCollider = GetComponent<Collider>();
        allLights = FindObjectsOfType<Light>();
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer >= checkInterval)
        {
            checkTimer = 0;
            bool isInLight = IsPlayerInLight();
            Debug.Log($"In Light: {isInLight} | SanitySystem.isInDarkness: {sanitySystem.isInDarkness}");
            sanitySystem.SetInDarkness(!isInLight);
        }
    }

    private bool IsPlayerInLight()
    {
        if (allLights == null || allLights.Length == 0)
        {
            Debug.LogWarning("No lights found in scene!");
            return false;
        }

        foreach (var light in allLights.OrderBy(l => Vector3.Distance(transform.position, l.transform.position)))
        {
            if (!light.enabled || light.intensity <= 0.01f)
            {
                Debug.DrawRay(transform.position, light.transform.position - transform.position, Color.gray);
                continue;
            }

            Vector3 lightPos = GetLightEffectivePosition(light);
            Vector3 playerPos = playerCollider.bounds.center;
            Vector3 directionToLight = (lightPos - playerPos).normalized;
            float distanceToLight = Vector3.Distance(playerPos, lightPos);

            Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                         Color.yellow, checkInterval);

            if (light.type != LightType.Directional)
            {
                float effectiveRange = light.range * rangeMultiplier;
                if (distanceToLight > effectiveRange)
                {
                    Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                                 Color.blue, checkInterval);
                    continue;
                }
            }

            if (light.type == LightType.Spot)
            {
                float angleToLight = Vector3.Angle(-directionToLight, light.transform.forward);
                if (angleToLight > light.spotAngle * 0.6f)
                {
                    Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                                 Color.magenta, checkInterval);
                    continue;
                }
            }

            bool isObstructed = Physics.Raycast(playerPos, directionToLight, distanceToLight, obstructionLayers);

            if (!isObstructed)
            {
                if (light.type == LightType.Point || light.type == LightType.Spot)
                {
                    if (distanceToLight > light.range)
                    {
                        Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                                     Color.cyan, checkInterval);
                        continue;
                    }
                }

                Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                             Color.green, checkInterval);
                Debug.Log($"Valid light found: {light.name} (Type: {light.type})");
                return true;
            }
            else
            {
                Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                             Color.red, checkInterval);
            }
        }

        Debug.Log("No valid light sources found");
        return false;
    }

    private Vector3 GetLightEffectivePosition(Light light)
    {
        if (light.type == LightType.Directional)
        {
            return transform.position - light.transform.forward * 1000f;
        }
        return light.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        foreach (var light in allLights)
        {
            if (!light.enabled) continue;

            Vector3 lightPos = GetLightEffectivePosition(light);
            Vector3 playerPos = transform.position;
            float distance = Vector3.Distance(playerPos, lightPos);

            if (distance > light.range * rangeMultiplier) continue;

            Gizmos.color = Physics.Raycast(playerPos, (lightPos - playerPos).normalized, distance, obstructionLayers)
                ? Color.red
                : Color.green;
            Gizmos.DrawLine(playerPos, lightPos);
        }
    }
}