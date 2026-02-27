using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeTrialTimer : MonoBehaviour
{
    public TMP_Text Timer;
    bool startedCounting;
    float time = 0;
    private void OnEnable()
    {
        LevelStart.LevelStarted += OnLevelStarted;
    }
    private void OnDisable()
    {
        LevelStart.LevelStarted -= OnLevelStarted;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        Timer.enabled = PlayerInput.all.Count == 1;
    }
    void OnLevelStarted(float startTime)
    {
        if (PlayerInput.all.Count == 1)
        {
            time = 0;
            startedCounting = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (startedCounting) 
        { 
            time += Time.deltaTime;
            Timer.text = "Time: "+ time.ToString("0.00");
        }
    }
}
