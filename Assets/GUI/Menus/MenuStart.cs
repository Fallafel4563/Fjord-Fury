using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class MenuStart : MonoBehaviour
{
    #region Properties

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject playerAmountMenu;
    [SerializeField] GameObject levelSelectionMenu;
    [SerializeField] GameObject trophiesMenu;    
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsMenu;

    #endregion

    // Buttons on starting screen for start menu
    #region StartMenuButtons

    // Takes you to screen selecting story mode or local multiplayer
    public void OnPlayButton()
    {
        startMenu.SetActive(false);
        playerAmountMenu.SetActive(true);
    }

    // Takes you to trophies screen
    public void OnTrophiesButton()
    {
        startMenu.SetActive(false);
        trophiesMenu.SetActive(true);
    }

    // Takes you to settings screen
    public void OnOptionsButton()
    {
        startMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    // Takes you to credits screen
    public void OnCreditsButton()
    {
        startMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    // Shuts off the game
    public void OnQuitButton()
    {
        Application.Quit();
    }

    #endregion

    // Game specifications
    #region Play


    //choose game mode > 1 / 2 player select
    #region GameMode

    // Story mode button

    // VS mode button

    #endregion

    // Pick 1-player or 2-player > level select
    #region StoryOrLocalMultiplayer
    
    // 1-player button

    // 2-player button
    
    //will then take you to mode selectio
    
    #endregion

    //choose game mode
    #region GameMode

    // Story mode button

    // VS



    // Choose which level
    #region LevelSelection
    // choose level, press button to pick level (if not completed level before you can't select)
    #endregion

    #endregion

    // Trophies / achievements corresponding to level
    #region Trophies
    // how do they correspond in the game??
    #endregion

    #endregion

    // Scroll through credits
    #region Credits

    // closes credits screen
    public void OnCloseCreditsButton()
    {
        creditsMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    #endregion

}   