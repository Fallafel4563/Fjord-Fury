using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene. Please tag your camera as 'MainCamera'.");
            enabled = false; // Disable the script if no camera is found
            return;
        }
    }

    void LateUpdate()
    {
        // Ensure the sprite always faces the camera
        if (mainCamera != null)
        {
            // Make the sprite look at the camera's position
            transform.LookAt(mainCamera.transform.position);

            // Prevent tilting along the X and Z axes, keeping it upright
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}