using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextSceneLoading : MonoBehaviour
{
    public void LoadSceneCoroutine()
    {
        StartCoroutine(LoadNextScene());
    }
    

    IEnumerator LoadNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("");//Add level to be loaded inside parentheses.

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
