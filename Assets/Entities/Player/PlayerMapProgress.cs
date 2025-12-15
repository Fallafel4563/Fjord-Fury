using System.Data;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using System.Numerics;

public class PlayerMapProgress : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject[] playerProgressIcons; 

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        List<float> valuesList = PlacementText.DistancesAlongSpline.Values.ToList();
        
        foreach(KeyValuePair<int, float> positionPair in PlacementText.DistancesAlongSpline)
        {
            float xPosition = Mathf.Lerp(-900,900,positionPair.Value);
            playerProgressIcons[positionPair.Key].transform.localPosition = new UnityEngine.Vector3(xPosition, 0, 0); 
        }
    }
}
