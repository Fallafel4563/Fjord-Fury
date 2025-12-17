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
        furthestPlayerDistance = placementText.valuesList[0];
        rubberbandBoost = rubberbandCurve.Evaluate(furthestPlayerDistance - distance);

        forwardSpeedMultiplier.SetForwardSpeedMultiplier("Rubberband boost", 1f + rubberbandBoost);
        //Debug.LogFormat("Rubberband {0}", rubberbandBoost);
    }
}
