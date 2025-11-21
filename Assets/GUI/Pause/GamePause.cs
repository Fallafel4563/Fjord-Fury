using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GamePause : MonoBehaviour
{
    public TextMeshProUGUI PauseText;
    public TextMeshProUGUI TimerObject;
    public Button ResumeButton;
    public Button SettingsButton;
    public Button QuitButton;
    private bool isGamePaused;
    InputAction pauseGame;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimerObject.enabled = false;
        PauseText.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        isGamePaused = false;
        pauseGame = InputSystem.actions.FindAction("Pause");

    }

    // Update is called once per frame
    void Update()
    {
        if (pauseGame.WasPressedThisFrame())
        {
           if (isGamePaused)
           {
                ResumeGame();
           } 
           else
            {
                PauseGame();
            }
        }
        
    }

    public void PauseGame()
    {
        TimerObject.enabled = true;
        Time.timeScale = 0;
        isGamePaused = true;
        ResumeButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
        PauseText.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        TimerObject.enabled = false;
        Time.timeScale = 1;
        isGamePaused = false;
        ResumeButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        PauseText.gameObject.SetActive(false);
    }

    private void QuitGame()
    {

    }

    public void SettingsActive()
    {

    }
}
