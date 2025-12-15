using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectOptions : MonoBehaviour
{
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private GameObject characterPrefab;
    List<GameObject> characters = new List<GameObject>();

    [SerializeField] Transform[] boatPositions;

    private bool ready;
    private int currentCharacter = 0;
    private PlayerInput playerInput;

    public Action<int, PlayerSelectInfo> CharacterSelected;
    public Action<int> CharacterDeselected;
    public Action StartGame;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }


    private void Start()
    {
        for (int i = 0; i < characterPrefab.transform.childCount; i++)
        {
            characters.Add(Instantiate(characterPrefab, transform));
            characters[i].transform.localScale = Vector3.one * 30;

            for (int j = 0; j < characterPrefab.transform.childCount; j++)
            {
                characters[i].transform.GetChild(j).gameObject.SetActive(j == i);
            }
        }

        UpdateActiveCharacters();
    }

    void Update()
    {
        List<int> characterIndexes = GetActiveCharacters();
        

        for (int i = 0; i < characterIndexes.Count; i++)
        {
            characters[characterIndexes[i]].transform.eulerAngles = new Vector3(0, Time.time * 30, 0);
        }
    }


    public void OnAccept()
    {
        // Select the current character when the palyer isn't ready
        if (!ready)
        {
            SetReady(true);
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
            SetReady(false);
            readyText.text = "Choosing";
            // Tell character select menu that a player has deselected a character
            CharacterDeselected?.Invoke(playerInput.playerIndex);
        }
    }


    public void OnLeft()
    {
        ChangeCharacter(-1);
    }


    public void OnRight()
    {
        ChangeCharacter(1);
    }

    void ChangeCharacter(int direction)
    {
        // Don't change character when ready
        if (ready)
            return;
        
        // Decrease the current character index
        currentCharacter += direction;

        currentCharacter = WrapCharacterIndex(currentCharacter);    

        UpdateActiveCharacters();
    }

    int WrapCharacterIndex(int characterIndex)
    {
        // Wraps number to the end when it becomes lower or larger than the character's list count
        if (characterIndex < 0)
            characterIndex = characters.Count - 1;
        if (characterIndex >= characters.Count)
            characterIndex = 0;

        return characterIndex;
    }

    List<int> GetActiveCharacters()
    {
        
        int previousCharacter = WrapCharacterIndex(currentCharacter - 1);
        int nextCharacter = WrapCharacterIndex(currentCharacter + 1);

        return new List<int> ()
        {
            previousCharacter,
            currentCharacter,
            nextCharacter  
        };
    }

    private void UpdateActiveCharacters()
    {
        List<int> characterIndexes = GetActiveCharacters();
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].SetActive(characterIndexes.Contains(i));
        }

        for (int i = 0; i < characterIndexes.Count; i++)
        {
            characters[characterIndexes[i]].transform.position = boatPositions[i].position - Vector3.forward * 10;
        }
        characters[characterIndexes[0]].transform.localScale = Vector3.one * 15;
        characters[characterIndexes[1]].transform.localScale = Vector3.one * 30;
        characters[characterIndexes[2]].transform.localScale = Vector3.one * 15;
    }

    void SetReady(bool ready)
    {
        List<int> characterIndexes = GetActiveCharacters();
        characters[characterIndexes[0]].SetActive(!ready);
        characters[characterIndexes[2]].SetActive(!ready);
    }
}

public struct PlayerSelectInfo
{
    public int characterIndex;
    public float totalTimeSpent;
    public InputDevice inputDevice;
}