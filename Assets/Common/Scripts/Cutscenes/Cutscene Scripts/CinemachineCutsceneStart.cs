using UnityEngine;
using UnityEngine.Playables;

public class CinemachineCutsceneStart : MonoBehaviour
{
    public PlayableDirector PlayableDirector;
     
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayableDirector.Play();
        }
    }
}
