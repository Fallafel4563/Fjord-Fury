using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class VignetteController : MonoBehaviour
{
    [SerializeField] private Animator vignette;
    public PlayerInput playerInput;
    private int playerIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIndex = playerInput.user.index;
        vignette = GetComponent<Animator>();
        playerInput =GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VignetteOn()
    {
        vignette.Play("VignetteSpike", 0, 0f);

    }
}
