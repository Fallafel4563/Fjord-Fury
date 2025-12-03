using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectOptions : MonoBehaviour
{
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private List<Sprite> characters = new();

    private bool ready;
    private int currentCharacter = 0;
    private Image image;
    private PlayerInput playerInput;

    public Action<int, PlayerSelectInfo> CharacterSelected;
    public Action<int> CharacterDeselected;
    public Action StartGame;


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
        // Select the current character when the palyer isn't ready
        if (!ready)
        {
            ready = true;
            readyText.text = "Ready";
            // Create a data struct about what device is connected to which player and which character they chose
            PlayerSelectInfo playerSelectInfo = new()
            {
                characterIndex = currentCharacter,
                inputDevice = playerInput.devices[0],
            };

            // Send data struct to the character select menu
            CharacterSelected?.Invoke(playerInput.playerIndex, playerSelectInfo);
        }
        // Only allow the first player to start the game
        else if (playerInput.playerIndex == 0)
        {
            StartGame?.Invoke();
        }
    }


    public void OnCancel()
    {
        // Remove player when pressing cancel and the player hasn't choosen a character
        // Don't allow the first player to quit the game
        if (!ready && playerInput.playerIndex != 0)
            Destroy(gameObject);
        else // Deselect the current cahracter when pressing cancel and the player is ready
        {
            ready = false;
            readyText.text = "Choosing";
            // Tell character select menu that a player has deselected a character
            CharacterDeselected?.Invoke(playerInput.playerIndex);
        }
    }


    public void OnLeft()
    {
        // Don't change character when ready
        if (ready)
            return;
        
        // Decrease the current character index
        currentCharacter--;
        // Wrap currentCharacter to the end when it becomes lower than the chracters list count
        if (currentCharacter < 0)
            currentCharacter = characters.Count - 1;
        UpdateImage();
    }


    public void OnRight()
    {
        // Don't change character when ready
        if (ready)
            return;
        
        // Increase the current character index
        currentCharacter++;
        // Wrap currentCharacter to the beginning when it becomes larger than the chracters list count
        if (currentCharacter >= characters.Count)
            currentCharacter = 0;
        UpdateImage();
    }


    private void UpdateImage()
    {
        image.sprite = characters[currentCharacter];
    }
}

public struct PlayerSelectInfo
{
    public int characterIndex;
    public InputDevice inputDevice;
}