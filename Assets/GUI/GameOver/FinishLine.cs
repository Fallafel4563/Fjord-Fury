using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameOverState gameOverController;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameOverController.isOverFinishLine = true;
            gameOverController.StopTimer();
        }
    }
}
