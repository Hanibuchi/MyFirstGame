using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHandler : MonoBehaviour, ILightHandler
{
    [SerializeField] private float lightFadeInDuration = 0.5f;
    float targetIntensity = 0.3f; // 好きな明るさに調整
    Light2D _lightComponent;
    void Awake()
    {
        _lightComponent = GetComponent<Light2D>();
    }
    public void OnPlaced()
    {
        StartCoroutine(FadeInLight(_lightComponent));
    }

    private IEnumerator FadeInLight(Light2D light)
    {
        float time = 0f;
        float duration = lightFadeInDuration;

        while (time < duration)
        {
            time += Time.deltaTime;
            light.intensity = Mathf.Lerp(0f, targetIntensity, time / duration);
            yield return null;
        }

        light.intensity = targetIntensity;
    }
}
