using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextSceneLoading : MonoBehaviour
{
    public string SceneToLoad;
    bool loading = false;
    public int playerCount;
    public void LoadSceneCoroutine()
    {
        playerCount++;
        if (!loading)
        {
        StartCoroutine(LoadNextScene());
            loading = true;
        }
    }
    

    IEnumerator LoadNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneToLoad);//Add level to be loaded inside parentheses.

        asyncLoad.allowSceneActivation = false;

         while (!asyncLoad.isDone)
         {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                
            }
            yield return null;
         }
    }
}
