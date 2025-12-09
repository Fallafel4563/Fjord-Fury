using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownEvent : MonoBehaviour
{
    [SerializeField] private Image countdownImage;

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
        StartCoroutine(ChangeCountDownImage(sprite));
    }


    private IEnumerator ChangeCountDownImage(Sprite sprite)
    {
        countdownImage.gameObject.SetActive(true);
        countdownImage.sprite = sprite;
        yield return new WaitForSeconds(0.75f);
        countdownImage.gameObject.SetActive(false);
    }
}