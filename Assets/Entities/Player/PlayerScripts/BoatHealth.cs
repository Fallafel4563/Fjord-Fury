using UnityEngine;
using TMPro;

public class BoatHealth : MonoBehaviour
{
    #region Properties

    // The player's health at the start
    [SerializeField] private int healthInitial = 3;
    // The player's health right now
    private int healthCurrent;
    // Reference to the UI Text element to display health

    [SerializeField] private GameEvent PlayerHealthUpdated;
    [SerializeField] private GameEventFloatData PlayerHealthAmount;
        
    #endregion

    #region Initialisation methods

    // Initialises this component
    void Start()
    {
        // Initialiase the player's current health
        ResetHealth();
        UpdateHealthEvent();
    }

    // Sets the player's current health back to its initial value
    public void ResetHealth()
    {
        // Reset the player's current health
        healthCurrent = healthInitial;
    }

    #endregion

    #region Gameplay methods

    // Reduces the player's current health
    // (NB: Call this if hit by enemy, activated trap, etc)
    public void TakeDamage(int damageAmount)
    {
        // Deduct the provided damage amount from the player's current health
        healthCurrent -= damageAmount;
        UpdateHealthEvent();
        // If the player has no health left now
        if (healthCurrent <= 0)
        {
            // Kill the player
            Destroy(gameObject);
            //change to die event trigger

        }
    }

    private void UpdateHealthEvent()
    {
        PlayerHealthAmount.data = healthCurrent;
        PlayerHealthUpdated.TriggerEvent();
    }

    // Increase the player's current health
    // (NB: Call this if picked up potion, herb, etc)
    public void Heal(int healAmount)
    {
        // Add the provided heal amount to the player's current health
        healthCurrent += healAmount;
        UpdateHealthEvent();

        // If the player has too much health now
        if (healthCurrent > healthInitial)
        {
            // Reset the player's current health
            ResetHealth();
        }
    }

    // Updates the UI Text element with the current health


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacles"))
        {
            Debug.Log("Player has taken damage!");
            TakeDamage(1);
        }
}
    
    #endregion
}
