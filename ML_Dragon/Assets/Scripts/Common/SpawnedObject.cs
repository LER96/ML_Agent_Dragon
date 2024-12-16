using UnityEngine;

public class SpawnedObject : MonoBehaviour
{
    private ObjectSpawner spawner;

    // This method is called to pass the spawner reference to the spawned object
    public void Initialize(ObjectSpawner spawner)
    {
        this.spawner = spawner;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (spawner != null)
        {
            spawner.RemoveInstance(gameObject);
        }
    }
}
