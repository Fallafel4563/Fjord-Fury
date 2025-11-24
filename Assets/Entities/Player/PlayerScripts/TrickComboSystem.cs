using System.Collections.Generic;
using UnityEngine;

public class TrickComboSystem : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public ForwardSpeedMultiplier forwardSpeedMultiplier;

    public int combo = 0;
    public int trickScore = 0;
    public bool performingTrick = false;


    public float inputBuffer = 0f;
    public float inputBufferDefault = 0.2f;


    public List<string> trickList = new();
    public int trickIndex = 0;
    public Dictionary<string, int> tableOfTricks = new();
    public string[] numberList = new string[] {"", "Double", "Trollple", "Quadtrollple", "Quintrollple", "Sextrollple", "Trolltastic", "Trolltacular"};



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
            StartTrick();
        }
    }



    private void StartTrick()
    {
        inputBuffer = 0f;
        performingTrick = true;
        combo++;

        // Get trick from trick list
        trickIndex = Random.Range(0, trickList.Count);
        string trickName = trickList[trickIndex];

        if (tableOfTricks.ContainsKey(trickName))
        {
            tableOfTricks[trickName] += 1;
        }
        else
        {
            tableOfTricks.Add(trickName, 1);
        }

        // Play trick animation
    }



    public void OnLanded()
    {
        //
    }


    public void OnJumped()
    {
        //
    }
}
