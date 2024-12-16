using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;        // The target to follow
    public Vector3 offset;          // The offset at which the object will follow the target
    public float smoothSpeed = 0.125f;  // The speed of the smooth follow

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Calculate the desired position with the offset
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the position of the object
        transform.position = smoothedPosition;
    }
}
