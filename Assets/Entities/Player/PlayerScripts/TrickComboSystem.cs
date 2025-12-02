using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TrickComboSystem : MonoBehaviour
{
    public Animator animator;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;
    [HideInInspector] public BoatMovementAnims boatMovementAnims;

    [HideInInspector] public int combo = 0;
    [HideInInspector] public int trickScore = 0;
    [HideInInspector] public bool performingTrick = false;


    public SpeedMultiplierCurve ImmediateComboBoostCurve;
    [HideInInspector] public float inputBuffer = 0f;
    [HideInInspector] public float inputBufferDefault = 0.2f;


    public float boostDuration = 5f;
    [HideInInspector] public float speedValue = 0f;


    public List<string> trickList = new();
    [HideInInspector] public int trickIndex = 0;
    [HideInInspector] public string trickName = "";
    [HideInInspector] public Dictionary<string, int> tableOfTricks = new();

    private string[] numberList = new string[] {"", "Double", "Trollple", "Quadtrollple", "Quintrollple", "Sextrollple", "Trolltastic", "Trolltacular"};


    private float boostValue = 0f;
    private List<string> tricks = new();

    public UnityEvent FailedTrick;
    public UnityEvent SucceededTrick;
    public Action<string> TrickScoreUpdated;


    // FIX: These are only here for because we don't have animations implemented yet
    private float tmp_trickDuration = 0.5f;
    private float tmp_trickTime = 0f;



    private void Update()
    {
        // Reduce inputBuffer time
        if (inputBuffer > 0f)
        {
            inputBuffer -= Time.deltaTime;
        }

        // Start trick when pressing 
        if (inputBuffer > 0f && !performingTrick && !playerMovement.isGrounded)
        {
            StartTrick(false);
        }


        if (boostValue > 0 && playerMovement.isGrounded)
        {
            boostValue -= Time.deltaTime;
            if (boostValue <= 0f)
                EndComboBoost();
        }

        // Fix: Finish trick after a short while (Replace with an event in the animation system that is triggered when a trick animation has finished)
        if (tmp_trickTime > 0)
        {
            tmp_trickTime -= Time.deltaTime;
            if (tmp_trickTime <= 0)
            {
                OnTrickAnimationFinished();
            }
        }
    }


    private void StartTrick(bool dashTrick)
    {
        performingTrick = true;
        combo++;

        // TODO: Set boost meters high value to bost time (UI)
        // TODO: Set boost meters value to boost time (UI)
        boostValue = boostDuration;

        // TODO: Player trick sound


        string currentTrickName;
        if (dashTrick)
        {
            // TODO: Trigger "Left/Right Dodge trick" animator event
            trickScore += 20;

            currentTrickName = "Dodge";
        }
        else
        {
            trickScore += 10;

            trickIndex = UnityEngine.Random.Range(0, trickList.Count);
            currentTrickName = trickList[trickIndex];
            boatMovementAnims.TrickAnim();
        }

        if (tableOfTricks.ContainsKey(currentTrickName))
            tableOfTricks[currentTrickName] += 1;
        else
            tableOfTricks.Add(currentTrickName, 0);

        //
        tmp_trickTime = tmp_trickDuration;

        // Get the long tricks name
        trickName = GetTrickName();

        // DONE: Update score text to show trickScore + "x" + combo
        TrickScoreUpdated?.Invoke(string.Format("{0} x {1}", trickScore, combo));
        Debug.LogFormat("Speed value {0}, Speed mult {1}", speedValue, forwardSpeedMultiplier.GetTotalMultiplierValue());
    }


    public void FailTrick()
    {
        // TODO: Trigger failed trick sound
        EndComboBoost();

        // TODO: Set animator trigget for failing trick
        FailedTrick.Invoke();

        Debug.Log("FAILED TRICK");
        Debug.LogFormat("Speed vaue {0}, Mult {1}", speedValue, forwardSpeedMultiplier.GetTotalMultiplierValue());
    }


    private void SucceedTrick()
    {
        // TODO: Send trickScore * Combo to score system

        speedValue += trickScore / 500f;

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", speedValue, ImmediateComboBoostCurve);

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("LongComboBoost", speedValue);

        // TODO: Start playing the boost sound
        // TODO: Add camera shake when boosting
        // TODO: Show boost particles

        SucceededTrick.Invoke();
    }


    private void EndComboBoost()
    {
        // TODO: Set boostMaxValue to 0
        // TODO: Hide boost meter

        combo = 0;
        trickScore = 0;
        speedValue = 0f;

        performingTrick = false;
        trickName = "";

        tableOfTricks.Clear();

        // TODO: Stop playing sound

        // Stop boost from affecting the movement
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ImmediateComboBoost", 0f);
        forwardSpeedMultiplier.SetForwardSpeedMultiplier("LongComboBoost", 0f);
    }



    // Get the long text that will be displayed on the boost bar
    private string GetTrickName()
    {
        string tricksName = "";
        for (int i = 0; i < tableOfTricks.Count; i++)
        {
            var item = tableOfTricks.ElementAt(i);
            string key = item.Key;
            int value = item.Value;

            string number = numberList[value];
            tricksName += number + " " + key + ",";
        }
        return tricksName;
    }



    public void OnDashed()
    {
        if (!playerMovement.isGrounded)
            StartTrick(true);
    }


    public void OnTrickAnimationFinished()
    {
        performingTrick = false;
    }


    public void OnLanded()
    {
        if (performingTrick)
        {
            FailTrick();
        }
        else if (combo > 0)
        {
            SucceedTrick();
        }

        // Clear tricks list. (To reset the trick text displayed on the UI)
        tricks.Clear();
    }


    public void OnRespawnStarted()
    {
        // Fail trick when respawning
        FailTrick();
    }
}
