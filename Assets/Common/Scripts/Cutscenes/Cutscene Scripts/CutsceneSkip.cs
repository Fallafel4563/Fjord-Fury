using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutsceneSkip : MonoBehaviour
{
    public PlayableDirector CutscenePlayableDirector;
    public PlayableDirector SkipLoading;
    public Button SkipButton;
    
    public void SkipCutscene()
    {
        CutscenePlayableDirector.Stop();
        SkipLoading.Play();
        
    }
}
