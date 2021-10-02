using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    
    
    [Range(0.0f, 1.0f)]
    [SerializeField] private float globalTime;
    public float fullDayLength;
    public float startTime = 0.4f;
    private float skyBoxLerpValue;
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
    
    public Material skyboxDay;
    public Material skyboxNight;
    public Material skyBoxNightStars;
    public Material skyDawn;

    [Header("Other Lighting")] 
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;
        
    
    Material skyboxMaterial;
    // Start is called before the first frame update
    void Start()
    {
        if (skyboxDay != null && skyboxNight != null)
        {
            skyboxMaterial = new Material(skyboxDay);
        }
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
        
        //Skybox
        if (skyboxDay != null && skyboxNight != null)
        {

            // LERP FORMULA -> time - minimum condition time / difference between max & min of condition
            if (globalTime < 0.3 && globalTime > 0.2)
            {
                // DAWN FROM NIGHT STARS
                skyboxMaterial.Lerp(skyBoxNightStars, skyDawn, (globalTime - 0.2f)/0.1f);
                RenderSettings.skybox = skyboxMaterial;
            } else if (globalTime < 0.35 && globalTime > 0.3)
            {
                // DAY FROM DAWN
                skyboxMaterial.Lerp(skyDawn, skyboxDay, (globalTime - 0.3f)/0.05f);
                RenderSettings.skybox = skyboxMaterial;
            } else if (globalTime < 0.75 && globalTime > 0.35)
            {
                // NIGHT NO STARS FROM MORNING
                //skyboxMaterial.Lerp(skyboxDay, skyboxNight, (globalTime - 0.35f)/0.25f);
                //RenderSettings.skybox = skyboxMaterial;
            } else if (globalTime < 0.9 && globalTime > 0.75)
            {
                // NIGHT NO STARS FROM MORNING
                skyboxMaterial.Lerp(skyboxDay, skyboxNight, (globalTime - 0.75f)/0.15f);
                RenderSettings.skybox = skyboxMaterial;
            } else if (globalTime > 0.9)
            {
                // NIGHT STARS
                skyboxMaterial.Lerp(skyboxNight, skyBoxNightStars, (globalTime - 0.9f)/0.1f);
                RenderSettings.skybox = skyboxMaterial;
            }
        }
        


            // light rotation
        sun.transform.eulerAngles = (globalTime - 0.25f) * noon * 4.0f;
        moon.transform.eulerAngles = (globalTime - 0.75f) * noon * 4.0f;
        
        //light intensity
        sun.intensity = sunIntensity.Evaluate(globalTime);
        moon.intensity = moonIntensity.Evaluate(globalTime);

        // change colors
        sun.color = sunColor.Evaluate(globalTime);
        moon.color = moonColor.Evaluate(globalTime);
        

      //  skyboxDay.Get<>();
        
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
