using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera2D; // Reference to the 2D camera
    public Camera camera3D; // Reference to the 3D camera

    private bool isUsing3D = false; // Current state of which camera is active

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle the state
            isUsing3D = !isUsing3D;

            // Enable the appropriate camera based on the current state
            camera2D.enabled = !isUsing3D;
            camera3D.enabled = isUsing3D;
        }
    }
}
