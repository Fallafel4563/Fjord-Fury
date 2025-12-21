using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    public static LoadScreen Instance;
    [SerializeField] private Animator animator;
    [SerializeField] RectTransform loadIconRect;

    [SerializeField] AnimationCurve blackOutCurve;
    [SerializeField] AnimationCurve revealCurve;

    AnimationCurve currentCurve;
    float animationTime = 0;

    bool loaded = false;
    string sceneToLoad = "";

    void Awake()
    {
        // Make this a singleton
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }


    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks or errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void StartLoad(string sceneName)
    {
        loadIconRect.gameObject.SetActive(false);
        loadIconRect.gameObject.SetActive(true);
        sceneToLoad = sceneName;
        animator.Play("Blackout");
    }


    public void LoadNewScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneToLoad != "")
        {
            loadIconRect.gameObject.SetActive(false);
            loadIconRect.gameObject.SetActive(true);
            animator.Play("Reveal");
            sceneToLoad = "";
        }
    }
}
