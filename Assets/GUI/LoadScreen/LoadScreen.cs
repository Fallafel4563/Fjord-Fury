using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] RectTransform loadIconRect;

    [SerializeField] AnimationCurve blackOutCurve;
    [SerializeField] AnimationCurve revealCurve;

    AnimationCurve currentCurve;
    float animationTime = 0;

    bool loaded = false;
    string sceneToLoad = "";

    void Awake() {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start() {
        Load("valhalla");
    }

    public void Load(string sceneName) {
        sceneToLoad = sceneName;
    }

    void Update() {
        if (sceneToLoad == "" && animationTime >= 1)
            return;

        currentCurve = !loaded ? blackOutCurve : revealCurve;
        if (animationTime > .4f)
            return;
        
        animationTime += Time.deltaTime;

        if (animationTime >= 1) {
            if (!loaded)
            {
                SceneManager.LoadScene(sceneToLoad);
                return;
            }
            else {
                Destroy(gameObject);
                sceneToLoad = "";
            }
        }

        loadIconRect.sizeDelta = Vector2.one * 4000 * currentCurve.Evaluate(animationTime);
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks or errors
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
