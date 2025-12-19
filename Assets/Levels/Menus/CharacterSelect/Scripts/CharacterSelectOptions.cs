using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelectOptions : MonoBehaviour
{
    [SerializeField] private GameObject choosingSection;
    [SerializeField] private GameObject readySection;
    [SerializeField] private GameObject characterPrefab;
    List<GameObject> characters = new List<GameObject>();

    [SerializeField] Transform[] boatPositions;
    [SerializeField] GameObject[] selectArrows;

    [SerializeField] AnimationCurve spinCurve;
    [SerializeField] float spinCurveMultiplier = 10;

    private bool ready;
    private int currentCharacter = 0;
    private PlayerInput playerInput;

    public Action<int, PlayerSelectInfo> CharacterSelected;
    public Action<int> CharacterDeselected;
    public Action StartGame;

    float currentSpinTime;


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
        
        currentSpinTime += Time.deltaTime;
        for (int i = 0; i < characterIndexes.Count; i++)
        {
            characters[characterIndexes[i]].transform.eulerAngles = new Vector3(0, currentSpinTime * 30, 0);
        }
    }


    public void OnAccept()
    {
        // Select the current character when the palyer isn't ready
        if (!ready)
        {
            SetReady(true);
            // Create a data struct about what device is connected to which player and which character they chose
            PlayerSelectInfo playerSelectInfo = new()
            {
                characterIndex = currentCharacter,
                inputDevice = playerInput.devices[0],
            };

            // Send data struct to the character select menu
            CharacterSelected?.Invoke(playerInput.playerIndex, playerSelectInfo);
        }
        // Allow any player to start the game
        else
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
            // Tell character select menu that a player has deselected a character
            // If it isn't ready, it shouldn't use deselect, as this decrements active players and breaks the game
            if (ready)
                CharacterDeselected?.Invoke(playerInput.playerIndex);
            else // Unready player presses back? Go to level select
            {
                OnboardingManager.isActive = false;
                SceneManager.LoadScene("LevelSelect");

            }
                
                
            SetReady(false);
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
            characters[characterIndexes[i]].transform.localScale = boatPositions[i].localScale * 30;
        }
    }

    void SetReady(bool isReady)
    {
        ready = isReady;
        List<int> characterIndexes = GetActiveCharacters();
        characters[characterIndexes[0]].SetActive(!isReady);
        characters[characterIndexes[2]].SetActive(!isReady);
        
        choosingSection.SetActive(!isReady);
        readySection.SetActive(isReady);

        if (isReady)
        {
            StartCoroutine(SillySpin());
        }
    }

    IEnumerator SillySpin()
    {
        float currentAnimationTime = 0;
        while (currentAnimationTime < 1)
        {
            currentAnimationTime += Time.deltaTime;
            currentSpinTime += spinCurve.Evaluate(currentAnimationTime) * spinCurveMultiplier * Time.deltaTime;
            yield return null;
        }
    }
}

public struct PlayerSelectInfo
{
    public int characterIndex;
    public float totalTimeSpent;
    public InputDevice inputDevice;
}