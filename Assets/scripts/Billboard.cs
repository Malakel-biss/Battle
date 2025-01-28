using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        // Make sure the camera is set. If not, try to get the main camera.
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return; // If there's still no camera, exit to avoid errors.
        }

        // Rotate the health bar to face the camera.
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }
}

