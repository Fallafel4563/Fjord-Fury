using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    public Image PauseText;
    public TextMeshProUGUI TimerObject;
    public RawImage backgroundPanel;
    public RawImage pausedBackground;
    public Image darkBackground;
    public Button ResumeButton;
    public Button OptionsButton;
    public Button QuitButton;
    private bool isGamePaused;
    InputAction pauseGame;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimerObject.enabled = false;
        PauseText.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        OptionsButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        backgroundPanel.gameObject.SetActive(false);
        pausedBackground.gameObject.SetActive(false);
        darkBackground.gameObject.SetActive(false);
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
        OptionsButton.gameObject.SetActive(true);
        QuitButton.gameObject.SetActive(true);
        PauseText.gameObject.SetActive(true);
        backgroundPanel.gameObject.SetActive(true);
        pausedBackground.gameObject.SetActive(true);
        darkBackground.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ResumeButton.gameObject);
    }

    public void ResumeGame()
    {
        TimerObject.enabled = false;
        Time.timeScale = 1;
        isGamePaused = false;
        ResumeButton.gameObject.SetActive(false);
        OptionsButton.gameObject.SetActive(false);
        QuitButton.gameObject.SetActive(false);
        PauseText.gameObject.SetActive(false);
        backgroundPanel.gameObject.SetActive(false);
        pausedBackground.gameObject.SetActive(false);
        darkBackground.gameObject.SetActive(false);
         EventSystem.current.SetSelectedGameObject(null);
    }

    private void QuitGame()
    {
         Time.timeScale = 1;
    }

    public void SettingsActive()
    {

    }
}
