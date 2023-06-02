using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public void NextLevel()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        switch (buildIndex)
        {
            case 1:
                int scene = PlayerPrefs.GetInt("GameMode");
                SceneManager.LoadScene(scene);
                break;
            case 2:
                SceneManager.LoadScene(0);
                break;
            case 3:
                SceneManager.LoadScene(2);
                break;
            default:
                SceneManager.LoadScene(0);
                break;
        }
        
    }
}
