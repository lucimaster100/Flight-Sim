using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoffGoal : MonoBehaviour
{
    [SerializeField]
    float hightGoal;
    [SerializeField]
    Plane plane;
    [SerializeField]
    GameObject pauseScreen;
    [SerializeField]
    GameObject planeHud;

    private void Update()
    {
        if (plane.transform.position.y > hightGoal)
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
