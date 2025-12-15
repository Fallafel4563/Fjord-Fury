using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject options;
    [SerializeField] GameObject credits;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBack();
        }
        if (Gamepad.current != null && Gamepad.current.bButton.wasPressedThisFrame)
        {
            HandleBack();
        }
    }

    private void HandleBack()
    {
        if (options.activeSelf || credits.activeSelf)
        {
            options.SetActive(false);
            credits.SetActive(false);
            startMenu.SetActive(true);
        }

    }
}