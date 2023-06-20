using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GameModeButton(int selection)
    {
        PlayerPrefs.SetInt("GameMode", selection);
        SceneManager.LoadScene(1);
    }

    public void EndlessGameMode()
    {
        SceneManager.LoadScene(4);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
