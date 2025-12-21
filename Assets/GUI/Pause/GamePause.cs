using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    public Button ResumeButton;
    public GameObject pausePanel;
    public GameObject darkBackground;
    public NextSceneLoading nextSceneLoading;

    private bool isGamePaused;
    private InputAction pauseGame;


    void Start()
    {
        ResumeGame();
        pauseGame = InputSystem.actions.FindAction("Pause");
    }


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
        Cursor.visible = true;
        Time.timeScale = 0;
        isGamePaused = true;
        pausePanel.SetActive(true);
        darkBackground.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ResumeButton.gameObject);
    }


    public void ResumeGame()
    {
        Cursor.visible = false;
        Time.timeScale = 1;
        isGamePaused = false;
        pausePanel.SetActive(false);
        darkBackground.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }


    public void RestartLevel()
    {
        Time.timeScale = 1;
        string currentScene = SceneManager.GetActiveScene().name;
        nextSceneLoading.LoadSceneFromName(currentScene);
    }


    public void QuitToMenu()
    {
        Time.timeScale = 1;
        nextSceneLoading.LoadSceneFromName("MainMenu");
    }
}
