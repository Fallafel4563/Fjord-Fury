using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
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
    private Image placementImage;

    private int playerIndex;
    public float distanceAlongTrack;
    public static Dictionary<int, float> DistancesAlongSpline = new Dictionary<int, float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineDistance = GetComponent<SplineTrackDistance>();
        placementText = playerControllerRef.playerHud.placementText;
        //distanceSpline = GameObject.FindWithTag("Distance Tracker").GetComponent<SplineContainer>(); 
        playerIndex = playerInput.user.index;
        placementImage = playerControllerRef.playerHud.placementImage;
    }

    // Update is called once per frame
    void Update()
    {
        distanceAlongTrack = splineDistance.distanceAlongSpline;
        DistancesAlongSpline[playerInput.playerIndex] = distanceAlongTrack;

        Debug.Log(DistancesAlongSpline.Values.ToList());

        List<float> valuesList = DistancesAlongSpline.Values.ToList();
        valuesList.Sort();
        valuesList.Reverse();

        int Index = valuesList.IndexOf(splineDistance.distanceAlongSpline);
        Debug.Log(Index + 1 + placementSuffixes[Index] + " playerIndex " + playerInput.playerIndex);
        //Debug.Log(Index + 1 + placementImages[Index] + " playerIndex " + playerInput.playerIndex);



        placementText.text = Index + 1 + placementSuffixes[Index];
        placementImage.sprite = placementImages[Index];
        playerControllerRef.playerHud.SetFirstPlayerShine(Index + 1);
        /*
        string firstPlace = placementSuffixes[0];
        string secondPlace = placementSuffixes[1];
        string thirdPlace = placementSuffixes[2];
        string fourthPlace = placementSuffixes[3];

        Sprite firstPlaceImage = placementImages[0];
        Sprite secondPlaceImage = placementImages[1];
        Sprite thirdPlaceImage = placementImages[2];
        Sprite fourthPlaceImage = placementImages[3];
        */

    }

}
