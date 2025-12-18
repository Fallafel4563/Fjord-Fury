using UnityEngine;

public class Ruber : MonoBehaviour
{
    public ForwardSpeedMultiplier forwardSpeedMultiplier;
    public PlacementText placementText;

    public AnimationCurve rubberbandCurve;

    public float rubberbandBoost { get; private set; }
    public float furthestPlayerDistance { get; private set; }
    public float distance { get; private set; }


    private void FixedUpdate()
    {
        distance = placementText.distanceAlongTrack;
        if (placementText.valuesList.Count <= 0)
            return;
        
        furthestPlayerDistance = placementText.valuesList[0];
        rubberbandBoost = 1f+ rubberbandCurve.Evaluate(furthestPlayerDistance - distance);

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Rubberband boost", rubberbandBoost);
    }
}
