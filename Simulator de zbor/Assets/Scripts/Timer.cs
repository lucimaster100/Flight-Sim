using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{   
    [SerializeField]
    float timerDuration;
    [SerializeField]
    Text timerText;
    [SerializeField]
    GameObject pauseScreen;

    void Start()
    {
        timerDuration *= 60f;
    }


    void Update()
    {
        timerDuration -=Time.deltaTime;

        if (timerDuration <= 0)
        {
            timerDuration = 0;
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            enabled = false;
        }
        float minutes = Mathf.FloorToInt(timerDuration / 60f);
        float seconds = Mathf.FloorToInt(timerDuration % 60);

        timerText.text = string.Format("{00:00}:{1:00}", minutes, seconds);
    }
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
