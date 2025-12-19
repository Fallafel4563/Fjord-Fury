using UnityEngine;

public class CreditsAnimHandler : MonoBehaviour
{

    private Vector3 startPosition;
    
    public void startScrolling()
    {
        transform.localPosition = startPosition;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += Vector3.up*Time.deltaTime*100f;
    }
}
