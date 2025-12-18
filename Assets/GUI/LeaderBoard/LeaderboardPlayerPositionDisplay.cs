using TMPro;
using UnityEngine;

public class LeaderboardPlayerPositionDisplay : MonoBehaviour
{
    public TMP_Text indexText;
    public TMP_Text timeTakenText;
    public GameObject skins;

    public void UpdateDispaly(int index, float timeTaken, int characterIndex)
    {
        indexText.text = string.Format("Player {0}", index);
        timeTakenText.text = string.Format("{0} secs", timeTaken);
        
        // Hide all skins
        for (int i = 0; i < skins.transform.childCount; i++)
        {
            skins.transform.GetChild(i).gameObject.SetActive(false);
        }

        // Enable the selected character skin
        skins.transform.GetChild(characterIndex).gameObject.SetActive(true);
    }
}
