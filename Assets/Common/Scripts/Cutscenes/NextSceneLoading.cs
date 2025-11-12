using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextSceneLoading : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("");


         while (!asyncLoad.isDone)
         {
            yield return null; 
         }
    }
}
