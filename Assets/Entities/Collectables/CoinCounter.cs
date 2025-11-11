using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{
    public ScriptableObjectFloat coinsCollected;
    public TextMeshProUGUI counter;
    

    // Update is called once per frame
    void Update()
    {
        counter.text = coinsCollected.value.ToString();
    }
}
