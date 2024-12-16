using UnityEngine;

public abstract class BaseAgent : MonoBehaviour
{
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected Transform[] waypoints;
    
    protected int targetIndex;

    protected virtual void Start()
    {
    }

    protected abstract void MoveToNextWaypoint();

    protected void MoveTowardsTarget(Vector3 target)
    {
    }
}