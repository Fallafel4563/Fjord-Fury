using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoading : MonoBehaviour
{
    public string SceneToLoad;


    public void LoadScene()
    {
        PlacementText.DistancesAlongSpline.Clear();
        if (LoadScreen.Instance != null)
        {
            Time.timeScale = 1f;
            LoadScreen.Instance.StartLoad(SceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(SceneToLoad);
        }
    }
}
