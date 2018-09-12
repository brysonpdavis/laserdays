using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTransition : MonoBehaviour {

    private IEnumerator flipTransition;
    public float LaserIntensity;
    public Color LaserColor;
    public float RealIntensity;
    public Color realColor;
    private Light light;

    private void Start()
    {
        light = GetComponent<Light>();
        GameObject Player = Toolbox.Instance.GetPlayer();

        if (Player.layer == 15)
        {
            light.intensity = LaserIntensity;
            light.color = LaserColor;
        }
        else if (Player.layer == 16)
            light.intensity = RealIntensity;
            light.color = realColor;
    }


    public void Flip(bool direction, float duration)
    {
        if (direction) {
            flipTransition = flipTransitionRoutine(RealIntensity, realColor,  duration);
        }

        else 
            flipTransition = flipTransitionRoutine(LaserIntensity, LaserColor, duration);

        StopAllCoroutines();
        StartCoroutine(flipTransition);

    }

    private IEnumerator flipTransitionRoutine(float endpoint, Color endColor, float duration)
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

            yield return null;
        }
    }

}
