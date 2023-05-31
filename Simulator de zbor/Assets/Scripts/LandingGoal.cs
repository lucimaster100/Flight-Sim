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

    public bool pause;
    Vector3 velocity;

    private void Start()
    {
        pause = true;
    }
    private void Update()
    {
        velocity = plane.localVelocity;
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        if (velocity == Vector3.zero)
        {
            if (pause)
            {
                Time.timeScale = 0f;
            }
            planeHud.SetActive(false);
            pauseScreen.SetActive(true);
        }
    }
}
