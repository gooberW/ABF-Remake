using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class LightManager : MonoBehaviour
{
    private Light[] lights;
    public List<Light> ignoredLights;
    public bool isGeneratorOff = false;

    private int lightsOn = 0;
    private bool locked = false;

 
    public event Action<Light, bool> OnLightChanged;

    void Start()
    {

        lights = FindObjectsOfType<Light>();


        if (ignoredLights != null && ignoredLights.Count > 0)
        {
            List<Light> temp = new List<Light>();
            foreach (Light l in lights)
            {
                if (!ignoredLights.Contains(l))
                    temp.Add(l);
            }
            lights = temp.ToArray();
        }


        foreach (Light l in lights)
        {
            l.enabled = false;
            OnLightChanged?.Invoke(l, false);
        }

        lightsOn = 0;
        locked = false;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
            fused();
    }


    public void ToggleLight(Light lightToToggle)
    {
        if (lightToToggle == null) return;
        if (locked) return; 


        if (lightToToggle.enabled)
        {
            lightToToggle.enabled = false;
            lightsOn--;
        }
        else
        {
            lightToToggle.enabled = true;
            lightsOn++;
        }

        OnLightChanged?.Invoke(lightToToggle, lightToToggle.enabled);

        if (lightsOn >= 3)
        {
            TurnOffAllLights();
            locked = true;
            isGeneratorOff = true;
        }
    }

    void TurnOffAllLights()
    {
        foreach (Light l in lights)
        {
            if (l != null)
            {
                l.enabled = false;
                OnLightChanged?.Invoke(l, false); 
            }
        }

        lightsOn = 0;
    }


    public void fused()
    {
        locked = false;
    }
}