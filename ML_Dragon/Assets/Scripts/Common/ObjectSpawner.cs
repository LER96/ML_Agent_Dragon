using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The prefab to spawn.")]
    public GameObject PrefabToSpawn;

    [Tooltip("List of spawn points where the prefab can appear.")]
    public List<Transform> SpawnPoints = new List<Transform>();

    [Tooltip("Maximum number of active instances allowed.")]
    public int MaxActiveInstances = 2;

    private List<GameObject> activeInstances = new List<GameObject>();

    private void Start()
    {
        if (SpawnPoints.Count < MaxActiveInstances)
        {
            Debug.LogError("Number of spawn points must be greater than or equal to MaxActiveInstances!");
            return;
        }

        // Initial spawning of objects
        SpawnObjects();
    }

    private void Update()
    {
        // Maintain the desired number of active instances
        if (activeInstances.Count < MaxActiveInstances)
        {
            SpawnObjects();
        }
    }

    private void SpawnObjects()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(SpawnPoints);

        while (activeInstances.Count < MaxActiveInstances && availableSpawnPoints.Count > 0)
        {
            // Select a random spawn point
            Transform spawnPoint = GetRandomSpawnPoint(availableSpawnPoints);

            // Spawn the object at the selected spawn point
            if (spawnPoint != null)
            {
                GameObject instance = Instantiate(PrefabToSpawn, spawnPoint.position, Quaternion.identity,transform);
                instance.GetComponent<SpawnedObject>().Initialize(this);
                activeInstances.Add(instance);
                availableSpawnPoints.Remove(spawnPoint);
            }
        }
    }

    private Transform GetRandomSpawnPoint(List<Transform> availableSpawnPoints)
    {
        if (availableSpawnPoints.Count == 0) return null;
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        return availableSpawnPoints[randomIndex];
    }

    /// <summary>
    /// Removes a spawned instance from the active list and destroys it.
    /// </summary>
    /// <param name="instance">The instance to remove and destroy.</param>
    public void RemoveInstance(GameObject instance)
    {
        if (activeInstances.Contains(instance))
        {
            activeInstances.Remove(instance);
            Destroy(instance);
        }
    }
}
