using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    #region Properties

    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject controls;    

    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown; 

    Resolution[] resolutions;

    #endregion

    #region PLayerPrefs     

    void Start()
    {
        // loads fullscreen prefs
        // Default to fullscreen ON if no entry exists
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = isFullscreen;
        
        // loads graphics quality prefs
        int savedQuality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(savedQuality);
        
        //Resolution
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        // Load saved resolution index
        int savedResolutionIndex = PlayerPrefs.GetInt("Resolution", -1);

        if (savedResolutionIndex != -1)
        {
            // Apply saved resolution
            SetResolution(savedResolutionIndex);
            resolutionDropdown.value = savedResolutionIndex;
        }
        else
        {
            // No saved value yet → use current system resolution
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }

        resolutionDropdown.value = currentResolutionIndex;
        }

    resolutionDropdown.RefreshShownValue();    
    }

    #endregion

    public void OnExitOptionsButton()
    {
        optionsMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    #region Controls
    
    //show panel with image of controls
    public void OnControlsButton()
    {
        controls.SetActive(true);
    }

    public void OnExitControlsButton()
    {
        controls.SetActive(false);
    }
    
    #endregion

    #region Display

    #region Display; Fullscreen toggle

//    public void SetFullscreen (bool isFullscreen)
//    {
//        Screen.fullScreen = isFullscreen;
//    }

    public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;

            // Save to PlayerPrefs (1 = true, 0 = false)
            PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }

    #endregion

    #region  Display; Set graphics quality

//to change quality go to edit > project settings > quality
//for now there low, medium and high quality
//The quality settings there are not defined yet

//    public void SetQuality (int qualityIndex)
//    {
//        QualitySettings.SetQualityLevel(qualityIndex);
//    }


    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("Quality", qualityIndex);
        PlayerPrefs.Save();
    }

    #endregion

    #region Display; Set resolution

//    void Start()
//    {
//        resolutions = Screen.resolutions;
//        resolutionDropdown.ClearOptions();

//        List<string> options = new List<string>();
//        int currentResolutionIndex = 0;

//        for (int i = 0; i < resolutions.Length; i++)
//        {
//            string option = resolutions[i].width + " x " + resolutions[i].height;
//            options.Add(option);
//            if (resolutions[i].width == Screen.currentResolution.width &&
//                resolutions[i].height == Screen.currentResolution.height)
//            {
//                currentResolutionIndex = i;
//            }
//        }

//        resolutionDropdown.AddOptions(options);
//        resolutionDropdown.value = currentResolutionIndex;
//        resolutionDropdown.RefreshShownValue();
//    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        PlayerPrefs.Save();
    }

    #endregion

    #endregion

    #region  Accessibility

    #region Screen shake

    public void OnShakeToggleButton()
    {

    }

    #endregion



    #endregion

    //DOES THIS WORK WITH FMOD??
    //IT DOES NOT
    //AUDIO TAB
    #region Audio

    //Master volume
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    //Music volume

    //SFX volume

    //Dialouge volume

    #endregion

}
