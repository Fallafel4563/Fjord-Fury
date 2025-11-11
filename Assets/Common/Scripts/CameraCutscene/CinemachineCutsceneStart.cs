using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCutsceneStart : MonoBehaviour
{
     [SerializeField] private CinemachineCamera targetCamera;
     [SerializeField] private int InUsePriority = 10;
     [SerializeField] private int NotInUsePriority = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<CinemachineCamera>();
            {
                other.gameObject.SetActive(true);
                Debug.Log("StartedCutscene");
            }
        }
    }
}
