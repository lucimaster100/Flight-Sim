using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    Plane plane;
    AudioSource audioSource;

    public float minPitch;
    public float maxPitch;
    public float minVolume;
    public float maxVolume;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        plane = GetComponent<Plane>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!plane.Dead)
        {
            float throttle = plane.throttle;
            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, throttle);
            audioSource.volume = Mathf.Lerp(minVolume, maxVolume, throttle);
        }
        else
        {
            audioSource.pitch = 0;
        }
    }
}
