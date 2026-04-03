using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLightFlicker : MonoBehaviour
{
    private Light fireLight;

    [Header("Impostazioni Fuoco")]
    public float minIntensity = 1.0f;
    public float maxIntensity = 2.5f;
    public float flickerSpeed = 5.0f;

    private float randomOffset;

    void Start()
    {
        fireLight = GetComponent<Light>();
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}