using TMPro;
using UnityEngine;

public class LeaderboardPlayerPositionDisplay : MonoBehaviour
{
    public TMP_Text indexText;
    public TMP_Text timeTakenText;


    public void UpdateDispaly(int index, float timeTaken)
    {
        indexText.text = string.Format("Player {0}", index);
        timeTakenText.text = string.Format("{0} secs", timeTaken);
    }
}
