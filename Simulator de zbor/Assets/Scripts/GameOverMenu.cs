using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField]
    TakeoffGoal takeoff;
   public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void NextLevel()
    {
        int scene = PlayerPrefs.GetInt("GameMode");
        takeoff.pause = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }

}
