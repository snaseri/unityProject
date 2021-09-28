using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //References
    [SerializeField ]private Light DirectionalLight;
    [SerializeField ]private LightingPreset Preset;
    //Vars
    [SerializeField, Range(0, 24)] private float TimeOfDay;
    // Start is called before the first frame update

    private void UpdateLighting(float timePrecent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePrecent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePrecent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePrecent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePrecent * 360f) - 90f, 170f, 0));
        }

    }

    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();

            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= 24;
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }
}
