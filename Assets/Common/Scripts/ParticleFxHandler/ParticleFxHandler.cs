using UnityEngine;

public class ParticleFxHandler : MonoBehaviour
{
    [System.Serializable]
    public class ParticleTypes
    {
        public string actionName;            // Name of player action
        public ParticleSystem particleSystem; // The particle system to play for said action
    }

    [Header("Particle Systems")]
    public ParticleTypes[] particleTypes;

    
    
    /// Plays the particle system associated with the given action.
   
    public void PlayActionParticles(string action)
    {
        foreach (var ap in particleTypes)
        {
            if (ap.actionName.Equals(action, System.StringComparison.OrdinalIgnoreCase))
            {
                if (ap.particleSystem != null)
                {
                    ap.particleSystem.Play();
                    return;
                }
            }
        }

        Debug.LogWarning($"[PlayerParticleHandler] No particle system found for action: {action}");
    }

   
    /// Stops the particle system associated with the given action.
    
    public void StopActionParticles(string action)
    {
        foreach (var ap in particleTypes)
        {
            if (ap.actionName.Equals(action, System.StringComparison.OrdinalIgnoreCase))
            {
                if (ap.particleSystem != null)
                {
                    ap.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    return;
                }
            }
        }
    }
}
