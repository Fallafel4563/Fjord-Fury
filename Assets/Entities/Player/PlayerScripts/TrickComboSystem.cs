using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;

public class TrickComboSystem : MonoBehaviour
{
    public Animator animator;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;
    [HideInInspector] public BoatMovementAnims boatMovementAnims;
    [SerializeField] private TrickAbilitySystem trickAbilitySystem;


    [HideInInspector] public bool performingTrick = false;
    [HideInInspector] public int combo = 0;
    [HideInInspector] public int trickScore = 0;
    [HideInInspector] public int trickIndex = 0;
    [HideInInspector] public int firstTrickIndex = 0;
    [HideInInspector] public int barIndex = 0;

    [HideInInspector] public float speedValue = 0f;
    private int shortBoost = 0;
    private int mediumBoost = 0;
    private int longBoost = 0;

    public float inputBufferDuration = 0.2f;
    public SpeedMultiplierCurve ImmediateComboBoostCurve;
    [HideInInspector] public float inputBuffer = 0f;

    public Action<bool> UpdateBoostMeterVisibility;
    public Action<int, int, int> UpdateBoostMeter;
    public Action ResetBoostMeter;
    public Action ResetTrickReaction;


    private void Update()
    {
        // Reduce inputBuffer time
        if (inputBuffer > 0f)
        {
            inputBuffer -= Time.deltaTime;
        }

        // Start trick
        if (inputBuffer > 0f && !performingTrick && !playerMovement.isGrounded)
        {
            StartTrick();
        }


        //if (boostValue > 0 && playerMovement.isGrounded)
        //{
        //    boostValue -= Time.deltaTime;
        //    if (boostValue <= 0f)
        //        EndComboBoost();
        //}
    }



    public void ActivateTrick(int trick)
    {
        trickIndex = trick;
        inputBuffer = inputBufferDuration;
    }


    private void StartTrick()
    {
        performingTrick = true;
        // TODO: Send trick sound to FMOD
        boatMovementAnims.TrickAnim();
    }


    public void OnTrickCompleted()
    {
        Debug.LogFormat("First {0}, Combo {1}", firstTrickIndex, combo);
        if (combo == 0 || firstTrickIndex == 0)
        {
            UpdateBoostMeterVisibility.Invoke(true);
            firstTrickIndex = trickIndex;
        }

        combo++;

        trickScore += 10 * trickIndex;

        performingTrick = false;

        switch (trickIndex)
        {
            case 1:
                shortBoost++;
                break;
            case 2:
                mediumBoost++;
                break;
            case 3:
                longBoost++;
                break;
        }

        barIndex++;
        if (barIndex >= 3)
            barIndex = 0;

        if (combo == 3)
        {
            // TODO: Send ability ready sound to FMOD
        }
        else
        {
            // TODO: Send Add charge sound to FMOD
        }

        Debug.LogFormat("First {0}, Combo {1}, Bar {2}", firstTrickIndex, combo, barIndex);
        UpdateBoostMeter?.Invoke(firstTrickIndex, combo, barIndex);
    }


    public void FailTrick()
    {
        EndComboBoost();
        // TODO: Trigger failed trick sound
        // TODO: Set animator trigget for failing trick
    }


    private void SucceedTrick()
    {
        speedValue += trickScore * combo / 300f;

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", 1f + speedValue, ImmediateComboBoostCurve);

        if (combo >= 3)
        {
            // TODO: Send ability activation to FMOD
            // TODO: Send ability success to ability system
                // First Trick index
                // Short boost
                // Medium boost
                // Long boost
        }
        else if (combo < 3)
        {
            trickAbilitySystem.SpawnAbility(firstTrickIndex, shortBoost, mediumBoost, longBoost);
            // TODO: Send ability failed to active to FMOD
            // TODO: Send fail ability to ability system
                // First trick index
        }

        UpdateBoostMeterVisibility?.Invoke(false);

        ResetSystemValues();

        // TODO: Something about ImmediateComboBoost.GetLength()

        // TODO: Start playing the boost sound
        // TODO: Add camera shake when boosting
        // TODO: Show boost particles
    }


    private void EndComboBoost()
    {
        UpdateBoostMeterVisibility?.Invoke(false);
        speedValue = 0f;
        combo = 0;
        trickScore = 0;
        performingTrick = false;

        ResetTrickReaction?.Invoke();

        ResetSystemValues();

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", 1f, ImmediateComboBoostCurve);

        // TODO: Stop playing sound
    }


    private void ResetSystemValues()
    {
        shortBoost = 0;
        mediumBoost = 0;
        longBoost = 0;
        combo = 0;
        trickIndex = 0;
        barIndex = 0;

        ResetBoostMeter?.Invoke();
    }


    public void OnLanded()
    {
        // Fail trick when landing while performing a trick
        if (performingTrick)
            FailTrick();
        else if (combo > 0)
            SucceedTrick();
    }


    public void OnHitObstacle()
    {
        FailTrick();
    }


    public void OnRespawnStarted()
    {
        FailTrick();
    }


    // Get the long text that will be displayed on the boost bar
    //private string GetTrickName()
    //{
    //    string tricksName = "";
    //    for (int i = 0; i < tableOfTricks.Count; i++)
    //    {
    //        var item = tableOfTricks.ElementAt(i);
    //        string key = item.Key;
    //        int value = item.Value;
    //
    //        string number = numberList[value];
    //        tricksName += number + " " + key + ",";
    //    }
    //    return tricksName;
    //}
}
