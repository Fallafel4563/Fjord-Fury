using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private List<GameObject> characterSelectPositions = new();


    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} joined", playerInput.playerIndex);
        playerInput.transform.SetParent(characterSelectPositions[playerInput.playerIndex].transform, false);
        playerInput.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }


    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.LogFormat("Player {0} left", playerInput.playerIndex);
    }
}
