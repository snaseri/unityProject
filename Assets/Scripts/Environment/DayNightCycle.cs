using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    
    [Range(0.0f, 1.0f)]
    public float time;

    private float globalTime;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon;


    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;
    
    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")] 
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;
        
    // Start is called before the first frame update
    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        globalTime = ((TimeManager.Hour * 60) + TimeManager.Minute) / 1440f;
        // increment time
        // time += timeRate * Time.deltaTime;
        //
        // if (time >= 1.0f)
        //     time = 0.0f;
        
        // light rotation
        sun.transform.eulerAngles = (globalTime - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (globalTime - 0.75f) * noon * 4.0f;
        
        //light intensity
        sun.intensity = sunIntensity.Evaluate(globalTime);
        moon.intensity = moonIntensity.Evaluate(globalTime);

        // change colors
        sun.color = sunColor.Evaluate(globalTime);
        moon.color = moonColor.Evaluate(globalTime);
        
        //enable / disable the sun
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);
        
        //enable / disable the moon
        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);
        
        //lighting and reflections intensity
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(globalTime);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(globalTime);
    }
}
