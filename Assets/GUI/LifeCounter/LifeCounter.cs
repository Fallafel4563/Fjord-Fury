using UnityEngine;
using TMPro;

public class LifeCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    public void PlayerTookDMG(GameEventData eventData)
    {
        if (eventData is GameEventFloatData floatData)
        {
            if (healthText != null)
            {
                healthText.text = "Health: " + floatData.data.ToString();
            }
        }
    }
}
