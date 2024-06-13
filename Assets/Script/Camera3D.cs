using UnityEngine;

public class Camera3D : MonoBehaviour
{
    public Transform target; // The target the camera should follow (typically the player)
    public Vector3 offset = new Vector3(-10, 10, -10); // Offset from the target
    public float smoothTime = 0.3f; // Damping time for the smooth transition

    private Vector3 velocity = Vector3.zero; // Velocity reference for smooth damping

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the target position from the offset and the target's position
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Ensure the camera is looking at the target
        transform.LookAt(target);
    }
}
