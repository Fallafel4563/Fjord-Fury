using UnityEngine;

public class MultiplayerCount : MonoBehaviour
{
    public void SetPlayers(int value)
    {
        MultiplayerPlayerSpawner.playerCount = value;
    }
}
