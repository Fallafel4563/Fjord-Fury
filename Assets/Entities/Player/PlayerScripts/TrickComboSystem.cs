using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class TrickComboSystem : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement playerMovement;
    public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public BoatMovementAnims boatMovementAnims;
    public TrickAbilitySystem trickAbilitySystem;


    public bool performingTrick { get; set; } = false;
    public float combo { get; set; } = 0f;
    public int trickScore { get; set; } = 0;
    public int trickIndex { get; set; } = 0;
    public int firstTrickIndex { get; set; } = 0;
    public int barIndex { get; set; } = 3;

    private int lengthBoost = 0;
    private int sizeBoost = 0;
    private int strengthBoost = 0;
    public int abilityActivationThreshold = 2;
    public List<string> abilityType = new List<string> { "Shroom", "Whirlwind", "Ram" };
    public List<string> boostType = new List<string> { "Longer", "Bigger", "Stronger" };

    public float inputBufferDuration = 0.2f;
    public SpeedMultiplierCurve ImmediateComboBoostCurve;
    public float inputBuffer { get; set; } = 0f;

    public Action<bool> UpdateBoostMeterVisibility;
    public Action<UpdateBoostMeterInfo> UpdateBoostMeter;
    public Action ResetBoostMeter;

    public UnityEvent TrickSucceed;
    public UnityEvent TrickFalied;


    private void Start()
    {
        UpdateBoostMeterVisibility?.Invoke(false);
    }


    private void Update()
    {
        // Reduce inputBuffer time
        if (inputBuffer > 0f)
        {
            inputBuffer -= Time.deltaTime;
        }

        // Start trick
        if (inputBuffer > 0f && !performingTrick && (!playerMovement.isGrounded || (playerMovement.isGrounded && playerMovement.currentTrack.isCircle)))
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

        if (playerMovement.isGrounded && playerMovement.currentTrack.isCircle)
            RailTrick();
        else
            NormalTrick();
        
        boatMovementAnims.TrickAnim();
    }


    private void NormalTrick()
    {
        if ((int)combo == 0)
        {
            UpdateBoostMeterVisibility?.Invoke(true);
            firstTrickIndex = trickIndex;
            WorldTextSpawner.instance.SpawnText(abilityType[trickIndex], transform.position, Color.black, transform);
            //Debug.Log("test test test");
        }
        else
        {
            WorldTextSpawner.instance.SpawnText(boostType[trickIndex], transform.position, Color.black, transform);
        }

        combo += 1f;

        switch (trickIndex)
        {
            case 0:
                lengthBoost++;
                // TODO: Send short trick sound to fmod
                break;
            case 1:
                sizeBoost++;
                // TODO: Send medium trick sound to fmod
                break;
            case 2:
                strengthBoost++;
                // TODO: Send long trick sound to fmod
                break;
        }

        barIndex++;
        if (barIndex >= 3)
            barIndex = 0;
        
        if ((int)combo == abilityActivationThreshold)
        {
            // TODO: Send ability ready to fmod
        }
        else
        {
            // TODO: Send ability charge to fmod
        }

        UpdateBoostMeterInfo updateBoostMeterInfo = new();
        updateBoostMeterInfo.combo = (int)combo;
        updateBoostMeterInfo.barIndex = barIndex;
        updateBoostMeterInfo.firstTrickIndex = firstTrickIndex;
        updateBoostMeterInfo.abilityActivationThreshold = abilityActivationThreshold;
        UpdateBoostMeter?.Invoke(updateBoostMeterInfo);

        // TODO: Send trick sound to FMOD
    }

    private void RailTrick()
    {
        combo = 0.5f;
    }


    public void OnTrickCompleted()
    {
        performingTrick = false;
    }


    public void FailTrick()
    {
        EndComboBoost();
        // TODO: Trigger failed trick sound
        // TODO: Set animator trigget for failing trick

        TrickFalied.Invoke();
    }


    private void SucceedTrick()
    {
        TriggerAbility();
        TriggerComboBoost();

        TrickSucceed.Invoke();
        UpdateBoostMeterVisibility?.Invoke(false);
    }


    private void TriggerAbility()
    {
        if ((int)combo >= abilityActivationThreshold)
        {
            // TODO: Send ability activation to FMOD
            trickAbilitySystem.SpawnAbility(firstTrickIndex, lengthBoost, sizeBoost, strengthBoost);
        }
        else
        {
            //trickAbilitySystem.SpawnAbilityFailed(firstTrickIndex);
        }
    }


    private void TriggerComboBoost()
    {
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", 1f + combo / 3, ImmediateComboBoostCurve);
        ResetSystemValues();

        // TODO: Fov, camera shake, 
    }


    private void EndComboBoost()
    {
        performingTrick = false;
        trickScore = 0;
        ResetSystemValues();
        UpdateBoostMeterVisibility?.Invoke(false);

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", 1f, ImmediateComboBoostCurve);

        // TODO: Stop boost playing sound
    }


    private void ResetSystemValues()
    {
        lengthBoost = 0;
        sizeBoost = 0;
        strengthBoost = 0;
        barIndex = 3;
        combo = 0;

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
