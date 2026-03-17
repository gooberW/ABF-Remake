using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LightSanitySystem : MonoBehaviour
{
    [Header("Settings")]
    public float checkInterval = 0.25f;
    public float rangeMultiplier = 1.5f;
    public LayerMask obstructionLayers;

    [Header("Light Layers")]
    public LayerMask realLightLayers;      // Lights that affect sanity
    public LayerMask fakeLightLayers;      // Lights that DON'T affect sanity (ambient/faint lighting)

    private SanitySystem sanitySystem;
    private float checkTimer;
    private Collider playerCollider;
    private Light[] allLights;

    // Optional: Track which lights are affecting sanity for debugging
    private List<Light> activeRealLights = new List<Light>();

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
            bool isInRealLight = IsPlayerInRealLight();
            bool isInAnyLight = IsPlayerInAnyLight();

            Debug.Log($"In Real Light: {isInRealLight} | In Any Light: {isInAnyLight} | SanitySystem.isInDarkness: {sanitySystem.isInDarkness}");

            // Only set darkness based on REAL lights
            sanitySystem.SetInDarkness(!isInRealLight);
        }
    }

    private bool IsPlayerInRealLight()
    {
        return IsPlayerInLightByLayer(realLightLayers, true);
    }

    private bool IsPlayerInAnyLight()
    {
        return IsPlayerInLightByLayer(realLightLayers | fakeLightLayers, false);
    }

    private bool IsPlayerInLightByLayer(LayerMask targetLayers, bool trackActiveLights)
    {
        if (allLights == null || allLights.Length == 0)
        {
            Debug.LogWarning("No lights found in scene!");
            return false;
        }

        if (trackActiveLights)
            activeRealLights.Clear();

        foreach (var light in allLights.OrderBy(l => Vector3.Distance(transform.position, l.transform.position)))
        {
            // Check if light is on target layers
            if (!IsLightInLayers(light, targetLayers))
            {
                Debug.DrawRay(transform.position, light.transform.position - transform.position, Color.gray);
                continue;
            }

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

            // Range check for non-directional lights
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

            // Spot light angle check
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

            // Obstruction check
            bool isObstructed = Physics.Raycast(playerPos, directionToLight, distanceToLight, obstructionLayers);

            if (!isObstructed)
            {
                // Final range check for point/spot lights
                if (light.type == LightType.Point || light.type == LightType.Spot)
                {
                    if (distanceToLight > light.range)
                    {
                        Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                                     Color.cyan, checkInterval);
                        continue;
                    }
                }

                // Light is valid and affecting the player
                Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                             Color.green, checkInterval);

                if (trackActiveLights)
                    activeRealLights.Add(light);

                Debug.Log($"Valid light found: {light.name} (Type: {light.type}, Layer: {LayerMask.LayerToName(light.gameObject.layer)})");
                return true;
            }
            else
            {
                Debug.DrawRay(playerPos, directionToLight * distanceToLight,
                             Color.red, checkInterval);
            }
        }

        return false;
    }

    private bool IsLightInLayers(Light light, LayerMask layers)
    {
        return ((1 << light.gameObject.layer) & layers) != 0;
    }

    private Vector3 GetLightEffectivePosition(Light light)
    {
        if (light.type == LightType.Directional)
        {
            return transform.position - light.transform.forward * 1000f;
        }
        return light.transform.position;
    }

    // Optional: Get information about active lights
    public List<Light> GetActiveRealLights()
    {
        return activeRealLights;
    }

    public bool IsPlayerInFakeLightOnly()
    {
        return IsPlayerInAnyLight() && !IsPlayerInRealLight();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        foreach (var light in allLights)
        {
            if (!light.enabled) continue;

            // Color code based on layer type
            bool isReal = IsLightInLayers(light, realLightLayers);
            bool isFake = IsLightInLayers(light, fakeLightLayers);

            if (!isReal && !isFake) continue;

            Vector3 lightPos = GetLightEffectivePosition(light);
            Vector3 playerPos = transform.position;
            float distance = Vector3.Distance(playerPos, lightPos);

            if (distance > light.range * rangeMultiplier) continue;

            // Set color based on light type and obstruction
            Color gizmoColor;
            if (Physics.Raycast(playerPos, (lightPos - playerPos).normalized, distance, obstructionLayers))
            {
                gizmoColor = Color.red; // Obstructed
            }
            else
            {
                gizmoColor = isReal ? Color.green : new Color(1f, 0.5f, 0f); // Green for real, Orange for fake
            }

            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(playerPos, lightPos);

            // Draw a small sphere at light position with appropriate color
            Gizmos.DrawSphere(lightPos, 0.2f);
        }
    }
}