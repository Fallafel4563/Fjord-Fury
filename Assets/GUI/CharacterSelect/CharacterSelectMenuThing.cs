using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectMenuThing : MonoBehaviour
{
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private List<Sprite> characters = new();

    private bool ready;
    private int currentCharacter = 0;
    private Image image;
    private PlayerInput playerInput;


    private void Awake()
    {
        image = GetComponent<Image>();
        playerInput = GetComponent<PlayerInput>();
    }


    private void Start()
    {
        UpdateImage();
    }


    public void OnAccept()
    {
        Debug.LogFormat("Player {0} pressed enter", playerInput.playerIndex);
        if (!ready)
        {
            ready = true;
            readyText.text = "Ready";
        }
    }


    public void OnCancel()
    {
        if (!ready)
            Destroy(gameObject);
        else
        {
            ready = false;
            readyText.text = "Choosing";
        }
        Debug.LogFormat("Player {0} pressed esc", playerInput.playerIndex);
    }


    public void OnLeft()
    {
        currentCharacter--;
        if (currentCharacter < 0)
            currentCharacter = characters.Count - 1;
        UpdateImage();
    }


    public void OnRight()
    {
        currentCharacter++;
        if (currentCharacter >= characters.Count)
            currentCharacter = 0;
        UpdateImage();
    }


    private void UpdateImage()
    {
        image.sprite = characters[currentCharacter];
    }
}
