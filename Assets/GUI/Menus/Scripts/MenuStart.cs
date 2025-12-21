using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MenuStart : MonoBehaviour
{
    #region Properties

    [SerializeField] GameObject startMenu;
    //[SerializeField] GameObject playerAmount;
    [SerializeField] GameObject optionsMenu;     
    [SerializeField] GameObject credits;
    [SerializeField] GameObject riverRally;
    [SerializeField] GameObject valHalla;

    public UnityEvent StartLevelSelect;
    public static bool startLevelSelect = false;
    #endregion

    private void Start()
    {
        Time.timeScale = 1.0f;
        if (startLevelSelect)
            StartLevelSelect.Invoke();
        startLevelSelect = false;
    }

    #region Play

    // Takes you to screen selecting how many players
    //public void OnStartButton()
    //{
    //    startMenu.SetActive(false);
    //    playerAmount.SetActive(true);
    //}


    public void OnStartButton()
    {
        SceneManager.LoadScene("CharacterSelectExampleScene");
    }

    public void onStartValhallaButton()
    {
        SceneManager.LoadScene("ValhallaCharacterSelect");
    }

    public void loadLevelSelect()
    {
        OnboardingManager.isActive = true;
        SceneManager.LoadScene("LevelSelect");
    }
    #endregion


    #region Options

    // Takes you to options screen
    public void OnOptionsButton()
    {   
        startMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnCloseOptionsButton()
    {
        optionsMenu.SetActive(false);
        startMenu.SetActive(true);        
    }
    #endregion


    #region Credits

    // Takes you to credits screen
    public void OnCreditsButton()
    {
        startMenu.SetActive(false);
        credits.SetActive(true);
    }

    // closes credits screen
    public void OnCloseCreditsButton()
    {
        credits.SetActive(false);
        startMenu.SetActive(true);
    }

    #endregion

 
    #region  Quit

    // Shuts off the game
    public void OnQuitButton()
    {
        Application.Quit();
    }

    #endregion


    //have always show mouse when active
    //show selected image as cursor




    public void OnValhallaShow()
    {
        riverRally.SetActive(false);
        valHalla.SetActive(true);        
    }

        public void OnRiverRallyShow()
    {
        valHalla.SetActive(false);
        riverRally.SetActive(true);        
    }
}   