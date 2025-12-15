using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    private GameObject currentPanel;

    [Header("Root Panel")]
    public GameObject mainMenuPanel;

    void Start()
    {
        if (mainMenuPanel == null)
        {
            Debug.LogError("Main Menu Panel not assigned!");
            return;
        }

//        SetAllPanelsInactive();
//        currentPanel = mainMenuPanel;
//        currentPanel.SetActive(true);
    }

    void Update()
    {
        if (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame)
        {
            GoBack();
        }

        // Keyboard back (Escape)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GoBack();
        }
    }


    public void OpenPanel(GameObject panelToOpen)
    {
        if (panelToOpen == null || panelToOpen == currentPanel)
            return;

        if (currentPanel != null)
        {
            panelHistory.Push(currentPanel);
            currentPanel.SetActive(false);
        }

        currentPanel = panelToOpen;
        currentPanel.SetActive(true);
    }


    public void GoBack()
    {
        if (panelHistory.Count == 0)
        {
            Debug.Log("No previous panel in history");
            return;
        }

        currentPanel.SetActive(false);
        currentPanel = panelHistory.Pop();
        currentPanel.SetActive(true);
    }

    /// <summary>
    /// Disable all panels under this canvas
    /// </summary>
//    private void SetAllPanelsInactive()
//    {
//        foreach (Transform child in mainMenuPanel.transform.parent)
//        {
//            child.gameObject.SetActive(false);
//        }
//    }
}