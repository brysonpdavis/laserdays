using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTransition : MonoBehaviour {

    private IEnumerator flipTransition;
    public float LaserIntensity;
    public Color LaserColor;
    public float laserShadowStrength;

    public float RealIntensity;
    public Color realColor;
    public float realShadowStrength;
    private Light light;

    private void Start()
    {
        light = GetComponent<Light>();
        GameObject Player = Toolbox.Instance.GetPlayer();

        if (Player.layer == 15)
        {
            light.intensity = LaserIntensity;
            light.color = LaserColor;
            light.shadowStrength = laserShadowStrength;
        }
        else if (Player.layer == 16)
            light.intensity = RealIntensity;
            light.color = realColor;
            light.shadowStrength = realShadowStrength;
    }


    public void Flip(bool direction, float duration)
    {
        if (direction) {
            flipTransition = flipTransitionRoutine(RealIntensity, realColor, realShadowStrength,  duration);
        }

        else 
            flipTransition = flipTransitionRoutine(LaserIntensity, LaserColor, laserShadowStrength, duration);

        StopAllCoroutines();
        StartCoroutine(flipTransition);

    }

    private IEnumerator flipTransitionRoutine(float endpoint, Color endColor, float endStrength, float duration)
    {

        float elapsedTime = 0;
        float actualDuration = duration*(1-(light.intensity / endpoint));
        float ratio = elapsedTime / actualDuration;
        float intenseStart = light.intensity;
        Color colorStart = light.color;
        float shadowStart = light.shadowStrength;
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(intenseStart, endpoint, ratio);
            light.intensity = value;

            Color colorValue = Color.Lerp(colorStart, endColor, ratio);
            light.color = colorValue;

            if (light.shadows != LightShadows.None)
            {
                float strength = Mathf.Lerp(shadowStart, endStrength, ratio);
                Mathf.Clamp01(strength);
                light.shadowStrength = strength;
            }
            yield return null;
        }
    }

}
