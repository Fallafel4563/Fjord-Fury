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

    void Start() {
        //StartLoad("valhalla");
    }


    public void StartLoad(string sceneName)
    {
        loadIconRect.gameObject.SetActive(false);
        loadIconRect.gameObject.SetActive(true);
        sceneToLoad = sceneName;
        animator.Play("Blackout");
    }

    public void Load(string sceneName) {
        sceneToLoad = sceneName;
    }


    public void LoadNewScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


    void Update()
    {
        return;
        if (sceneToLoad == "")
            return;

        currentCurve = !loaded ? blackOutCurve : revealCurve;
        //if (animationTime > .4f)
        //    return;
        
        animationTime += Time.deltaTime;

        if (animationTime >= 1) {
            if (!loaded)
            {
                SceneManager.LoadScene(sceneToLoad);
                return;
            }
            else {
                Destroy(gameObject);
                return;
            }
        }

        loadIconRect.sizeDelta = Vector2.one * 10000 * currentCurve.Evaluate(animationTime);
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks or errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        return;
        if (sceneToLoad != "")
            loaded = true;
        
        animationTime = 0;
        if (loadIconRect != null)
        {
            // Toggle the component to force a redraw/re-stencil
            loadIconRect.gameObject.SetActive(false);
            loadIconRect.gameObject.SetActive(true);
        }
    }
}
