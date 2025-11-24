using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameOverState : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public Button restartbutton;
    public float timeLeft = 5.0f;
    public bool isGameOver;
    public bool isOverFinishLine;
    public bool activeCountdown;
    [SerializeField] private Image screenFade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        restartbutton.gameObject.SetActive(false);
        isGameOver = false;
        activeCountdown = true;
        isOverFinishLine = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeCountdown == true)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <=0)
            {
                
                GameOver(); 
            }
            else
            {
                StopTimer();
            }
        }
        
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartbutton.gameObject.SetActive(true);
        isGameOver = true;
        activeCountdown = false;

    }

    public void StopTimer()
    {
        if (isOverFinishLine == true && isGameOver == false)
        {
            activeCountdown = false;
        }
    }

    public void ReloadThisScene()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitToMainMenu()
    {
        
    }
}
