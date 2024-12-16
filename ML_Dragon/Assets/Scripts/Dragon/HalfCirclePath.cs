using UnityEngine;

public class HalfCirclePath : MonoBehaviour
{
    public float radius = 5f; // Radius of the half-circle
    public int segments = 100; // Number of segments to draw the path
    public Transform objectToMove; // Reference to the object to move along the path
    public Vector3 localHorizontalAxis = Vector3.right; // Local direction of the horizontal axis of the circle
    public Vector3 localVerticalAxis = Vector3.up; // Local direction of the vertical axis of the circle

    void OnDrawGizmos()
    {
        // Draw the half-circle path in local space
        Gizmos.color = Color.green;

        Vector3 startPosition = transform.position + transform.TransformDirection(localHorizontalAxis) * -radius;
        Vector3 previousPoint = startPosition;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.PI * i / segments; // Angle for the current segment

            Vector3 localPoint = localHorizontalAxis * (-radius * Mathf.Cos(angle)) + localVerticalAxis * (radius * Mathf.Sin(angle));
            Vector3 worldPoint = transform.position + transform.TransformDirection(localPoint);

            Gizmos.DrawLine(previousPoint, worldPoint);
            previousPoint = worldPoint;
        }
    }

    public Vector3 GetPositionAlongPath(float value)
    {
        // Clamp the input value between -1 and 1
        value = Mathf.Clamp(value, -1f, 1f);

        // Calculate the x position based on the input value
        float x = radius * value;

        // Calculate the y position using the half-circle equation
        float y = Mathf.Sqrt(radius * radius - x * x);

        // Calculate the local position along the path
        Vector3 localPosition = localHorizontalAxis * x + localVerticalAxis * y;

        // Convert the local position to world space
        return transform.position + transform.TransformDirection(localPosition);
    }

    void Update()
    {
        if (objectToMove != null)
        {
            // Example usage: Move the object based on a value ranging from -1 to 1
            float value = Mathf.PingPong(Time.time, 2f) - 1f; // Example value that oscillates between -1 and 1
            objectToMove.position = GetPositionAlongPath(value);
        }
    }
}
