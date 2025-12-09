using UnityEngine;
using UnityEngine.UI;

public class CountdownEvent : MonoBehaviour
{
    [SerializeField] private RawImage countdownText;

    private void OnEnable()
    {
        LevelStart.UpdateCountDownImage += OnCountdownUpdate;
    }

    private void OnDisable()
    {
        LevelStart.UpdateCountDownImage -= OnCountdownUpdate;
    }

    private void OnCountdownUpdate(Sprite sprite)
    {
        countdownText.texture = sprite.texture;
        countdownText.enabled = true;
    }
}