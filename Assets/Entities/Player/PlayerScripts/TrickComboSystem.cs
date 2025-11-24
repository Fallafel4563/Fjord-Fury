using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TrickComboSystem : MonoBehaviour
{
    public Animator animator;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;

    [HideInInspector] public int combo = 0;
    [HideInInspector] public int trickScore = 0;
    [HideInInspector] public bool performingTrick = false;

    private int currentScore = 0;


    public SpeedMultiplierCurve comboMultiplierCurve;
    [HideInInspector] public float inputBuffer = 0f;
    [HideInInspector] public float inputBufferDefault = 0.2f;


    [HideInInspector] public float boostTime = 0f;
    [HideInInspector] public float speedValue = 0f;


    public List<string> trickList = new();
    [HideInInspector] public int trickIndex = 0;
    [HideInInspector] public string trickName = "";
    [HideInInspector] public Dictionary<string, int> tableOfTricks = new();

    private string[] numberList = new string[] {"", "Double", "Trollple", "Quadtrollple", "Quintrollple", "Sextrollple", "Trolltastic", "Trolltacular"};


    private List<string> tricks = new();

    public UnityEvent FailedTrick;
    public UnityEvent SucceededTrick;

    private float tmp_trickDuration = 0.5f;
    private float tmp_trickTime = 0f;



    private void Awake()
    {
        forwardSpeedMultiplier = GetComponent<ForwardSpeedMultiplier>();
    }



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

        if (tmp_trickTime > 0)
        {
            tmp_trickTime -= Time.deltaTime;
            if (tmp_trickTime <= 0)
            {
                OnTrickAnimationFinished();
            }
        }

        // Get the long tricks name
        trickName = GetTrickName();
    }


    private void StartTrick(bool dashTrick)
    {
        inputBuffer = 0f;
        performingTrick = true;
        combo++;

        string newTrickName = "";
        // Dash trick
        if (dashTrick)
        {
            trickScore = 20;
            newTrickName = "Dodge";
        }
        else // Regular trick
        {
            trickScore = 10;
            // Get random trick from trick list
            trickIndex = Random.Range(0, trickList.Count);
            newTrickName = trickList[trickIndex];
        }
        

        if (tableOfTricks.ContainsKey(newTrickName))
        {
            tableOfTricks[newTrickName] += 1;
        }
        else
        {
            tableOfTricks.Add(newTrickName, 0);
        }

        // Play trick animation
        //animator.SetInteger("Trick Index", trickIndex);
        //animator.SetTrigger("Regular Trick");

        //
        tmp_trickTime = tmp_trickDuration;
    }


    private void FailTrick()
    {
        combo = 0;
        currentScore = 0;
        boostTime = 0f;
        performingTrick = false;
        //TODO: Set trick text to "
        tableOfTricks.Clear();
        //TODO: Play failed trick sound
        //TODO: Set boost meter value to 0
        //TODO: Stop playing boost sound
        //animator.SetTrigger("Failed Trick");
        FailedTrick.Invoke();
    }


    private void SucceedTrick()
    {
        currentScore = combo * trickScore;
        //TODO: Send score to ScoreSystem

        boostTime += currentScore / 100;
        //TODO: Set boost meter's high value to boost time
        //TODO: Set boost meter's value to boost time

        speedValue += currentScore / 300;

        currentScore = 0;
        combo = 0;
        tableOfTricks.Clear();

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("ComboBoost", 1 + speedValue, comboMultiplierCurve);

        // While boosting
        //TODO: Play boost sound
        //TODO: Increase fov
        //TODO: Set boost meter's value to boost time
        //TODO: Spawn particles
        SucceededTrick.Invoke();
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
        Debug.Log(trickName);
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

        // Reset stuff
        tricks.Clear();
    }


    public void OnJumped()
    {
        //
    }
}
