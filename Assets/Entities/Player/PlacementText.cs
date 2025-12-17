using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlacementText : MonoBehaviour
{
    public PlayerInput playerInput;
    private TMP_Text placementText;
    public PlayerController playerControllerRef;
    //private SplineContainer distanceSpline;
    //public SplineTrackDistance distanceSpline;
    public List<string> placementSuffixes = new List<string>();
    public SplineTrackDistance splineDistance;
    public List<Sprite> placementImages = new List<Sprite>();

    public List<float> valuesList = new();

    private Image placementImage;
  
    private int playerIndex;
    public float distanceAlongTrack;
    public static Dictionary<int, float> DistancesAlongSpline = new Dictionary<int, float>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {  
        splineDistance = GetComponent<SplineTrackDistance>();
        placementText = playerControllerRef.playerHud.placementText;

        playerIndex = playerInput.user.index;
        placementImage = playerControllerRef.playerHud.placementImage;
    }

    // Update is called once per frame
    void Update()
    {
        distanceAlongTrack = splineDistance.distanceBetween0and1;
        DistancesAlongSpline[ playerInput.playerIndex] = distanceAlongTrack;

        //Debug.Log(DistancesAlongSpline.Values.ToList());

        valuesList = DistancesAlongSpline.Values.ToList();
        valuesList.Sort();
        valuesList.Reverse();

       int Index = valuesList.IndexOf(distanceAlongTrack);
      
       //placementText.text =Index + 1 + placementSuffixes[Index];
       placementImage.sprite = placementImages[Index];
       playerControllerRef.playerHud.SetFirstPlayerShine(Index + 1);    

      
    }

}
