using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointHandler : MonoBehaviour
{
    public Transform[] waypoints;

    void OnDrawGizmos()
    {
        if (waypoints != null)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoints[i].position, 0.5f);

                if (i < waypoints.Length - 1)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }
}
