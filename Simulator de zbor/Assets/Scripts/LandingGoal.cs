using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingGoal : MonoBehaviour
{
    [SerializeField]
    Plane plane;
    [SerializeField]
    GameObject pauseScreen;
    [SerializeField]
    GameObject planeHud;

    Vector3 velocity;


    private void Update()
    {
        velocity = plane.localVelocity;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        if (velocity == Vector3.zero && !plane.Dead)
        {
            Time.timeScale = 0f;
            planeHud.SetActive(false);
            pauseScreen.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
