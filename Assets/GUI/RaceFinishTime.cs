using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RaceFinishTime : MonoBehaviour
{
    public ScriptableObjectFloat finishTime;
    public TextMeshProUGUI timeDisplay;
    public bool isCounting;
    

    // Update is called once per frame
    void Update()
    {
        timeDisplay.text = "Time: "+ finishTime.value.ToString();
        if (isCounting)
        {
            finishTime.value += Time.deltaTime;   
        }
     
    }
    
    public void ResetTime()
    {
        finishTime.value = 0;
    }
}
