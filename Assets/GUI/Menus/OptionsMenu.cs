using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;


public class SettingsMenu : MonoBehaviour
{
    //ADD IT TO PLAYER PREFS SO IT IS CONSISTENT OVER SCENES

    #region Properties

    [SerializeField] GameObject tabSelection;
    [SerializeField] GameObject controlsTab;
    [SerializeField] GameObject displayTab;
    [SerializeField] GameObject accessibilityTab;
    [SerializeField] GameObject audioTab;
    [SerializeField] GameObject settings;

    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown; 

    Resolution[] resolutions;

    #endregion

    //Select which settings you want to go into
    #region  TabSelectionButtons

    //Open controls tab
    //Closes tab tab selection
    public void OnControlsButton()
    {
        tabSelection.SetActive(false);
        controlsTab.SetActive(true);
    }

    //Open display tab
    //Closes tab selection
    public void OnDisplayButton()
    {
        tabSelection.SetActive(false);
        displayTab.SetActive(true);
    }

    //Open Accesibility tab
    //Closes tab selection
    public void OnAccessibilityButton()
    {
        tabSelection.SetActive(false);
        accessibilityTab.SetActive(true);
    }

    //Open audio tab
    //Closes tab selection
    public void OnAudioButton()
    {
        tabSelection.SetActive(false);
        audioTab.SetActive(true);
    }

    //Close settings
    public void OnCloseSettingsButton()
    {
        settings.SetActive(false);
    }

    #endregion

    //Controls tab
    #region Controls    

    public void OnCloseControls()
    {
        controlsTab.SetActive(false);
    }

    #endregion

    //Display tab
    #region Display

    #region Display; Fullscreen toggle

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    #endregion

    #region  Display; Set graphics quality

    //to change quality go to edit > project settings > quality
    //for now there low, medium and high quality
    //The quality settings there are not defined yet
    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    #endregion

    //TBA
    #region Display; Brightness

    #endregion

    #region Display; Set resolution

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    #endregion

    #endregion

    //Accessibility tab
    #region  Accessibility

    //Subtitles??

    #region Screen shake

    public void OnShakeToggleButton()
    {

    }

    #endregion

    #region Text size

    public void OnTextSizeButton()
    {

    }

    #endregion

    #endregion

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
