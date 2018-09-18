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
        //int property = Shader.PropertyToID("_D7A8CF01");

        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            float value = Mathf.Lerp(light.intensity, endpoint, ratio);
            light.intensity = value;

            Color colorValue = Color.Lerp(light.color, endColor, ratio);
            light.color = colorValue;

            float strength = Mathf.Lerp(light.intensity, endStrength, ratio);
            light.shadowStrength = strength;

            yield return null;
        }
    }

}
