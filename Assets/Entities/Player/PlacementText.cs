using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlacementText : MonoBehaviour
{
    public PlayerInput playerInput;
    public TMP_Text placementText;
    //private SplineContainer distanceSpline;
    //public SplineTrackDistance distanceSpline;
    public List<string> placementSuffixes = new List<string>();
    public SplineTrackDistance splineDistance;
  
    private int playerIndex;
    public float distanceAlongTrack;
    public static Dictionary<int, float> DistancesAlongSpline = new Dictionary<int, float>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
        splineDistance = GetComponent<SplineTrackDistance>();
        //distanceSpline = GameObject.FindWithTag("Distance Tracker").GetComponent<SplineContainer>(); 
        playerIndex = playerInput.user.index;
    }

    // Update is called once per frame
    void Update()
    {
        distanceAlongTrack = splineDistance.distanceAlongSpline;
        DistancesAlongSpline[ playerInput.playerIndex] = distanceAlongTrack;

        Debug.Log(DistancesAlongSpline.Values.ToList());

        List<float> valuesList = DistancesAlongSpline.Values.ToList();
        valuesList.Sort();

       int Index = valuesList.IndexOf(splineDistance.distanceAlongSpline);
       Debug.Log(Index + 1 + placementSuffixes[Index] + " playerIndex " + playerInput.playerIndex);

      // float placement = Index;

        
       string firstPlace = placementSuffixes[0]; 
       string secondPlace = placementSuffixes[1]; 
       string thirdPlace = placementSuffixes[2]; 
       string fourthPlace = placementSuffixes[3]; 

       //DistancesAlongSpline[ PlayerInput.playerIndex] = distanceAlongTrack;
       //
       //placementText = placement + 1 + placementSuffixes[placement];
    }

}
