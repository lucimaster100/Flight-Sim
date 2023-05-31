using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeoffGoal : MonoBehaviour
{
    [SerializeField]
    Plane plane;
    [SerializeField]
    GameObject pauseScreen;
    [SerializeField]
    GameObject planeHud;

    public bool pause;

    private void Start()
    {
        pause = true;
    }
    private void Update()
    {
        if (plane.transform.position.y > 1000f)
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
