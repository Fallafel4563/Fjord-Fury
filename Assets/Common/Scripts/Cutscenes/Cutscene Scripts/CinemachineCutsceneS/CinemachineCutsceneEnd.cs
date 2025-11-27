using UnityEngine;
using UnityEngine.Playables;

public class CinemachineCutsceneEnd : MonoBehaviour
{
    public PlayableDirector PlayableDirector;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayableDirector.Stop();
            PlayableDirector.time = 0;
            PlayableDirector.Evaluate();
        }
    }
}
