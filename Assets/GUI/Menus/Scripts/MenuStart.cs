using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuStart : MonoBehaviour
{
    #region Properties

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject playerAmount;
    [SerializeField] GameObject optionsMenu;     
    [SerializeField] GameObject credits;

    #endregion


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
}   