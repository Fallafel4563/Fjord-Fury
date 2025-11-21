using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextSceneLoading : MonoBehaviour
{
    public string SceneToLoad;
    bool loading = false;
    public void LoadSceneCoroutine()
    {
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
