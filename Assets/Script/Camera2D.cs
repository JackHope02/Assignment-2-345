using UnityEngine;

public class Camera2D : MonoBehaviour
{
    public Transform target; // The target the camera should follow (typically the player)
    public Vector3 offset = new Vector3(0, 0, -10); // Offset from the target
    public float smoothTime = 0.3f; // Damping time for the smooth transition

    private Vector3 velocity = Vector3.zero; // Velocity reference for smooth damping

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position is the target's position plus the fixed offset
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, 0) + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Keep the camera's rotation fixed at its initial rotation (no tilt or turn)
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}