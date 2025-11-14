using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    float currentTime;
    public float startingTime = 10f;
    public float littleTime = 5f;
    public float noTime = 2f;


    [SerializeField] TMP_Text countdownText;
    [SerializeField] GameObject countdownText1;

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        // countdownText.text = currentTime.ToString("0");

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        countdownText.text = string.Format("{00:00}:{01:00}", minutes, seconds);

        if (currentTime <= 0)
        {
            currentTime = 0;

            //gameOver.SetActive(true);
            countdownText1.SetActive(false);
        }

        else if (currentTime <= noTime)
        {
            countdownText.color = Color.red;
        }

        else if (currentTime <= littleTime)
        {
            countdownText.color = Color.yellow;
        }

        else
        {
            countdownText.color = Color.white;
        }
    }
    
    //public void TriggerGameOver()
    //{
    //    gameOver.SetActive(true);
    //    {
    //        freeze.SetActive(false);
    //    }
    //}
}

