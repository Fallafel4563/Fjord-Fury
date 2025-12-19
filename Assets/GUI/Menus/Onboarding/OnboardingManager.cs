using UnityEngine;
using UnityEngine.Events;

public class OnboardingManager : MonoBehaviour
{
    [SerializeField] GameObject controls;
    [SerializeField] GameObject abilities;
    [SerializeField] GameObject tricks;

    public static bool isActive = true;

    private GameObject[] panels;  // Array to store the panels
    private int currentActiveObjectIndex = 0;

    public UnityEvent noMorePanels;
    void Start()
    {
        if (!isActive)
            noMorePanels.Invoke();

        // Initialize the array
        panels = new GameObject[] { controls, abilities, tricks };

        // Ensure only the first panel is active at start
        UpdateCurrentlyActiveObject();
    }

    public void WhenButtonClicked()
    {
        currentActiveObjectIndex++;
        

        // Wrap around if we go past the last panel
        if (currentActiveObjectIndex >= panels.Length)
        {
            noMorePanels.Invoke();
            currentActiveObjectIndex = panels.Length - 1; // stop at last panel
            // OR use 0 if you want it to loop back: currentActiveObjectIndex = 0;
        }

        UpdateCurrentlyActiveObject();
    }

    private void UpdateCurrentlyActiveObject()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == currentActiveObjectIndex);
        }
    }
}
