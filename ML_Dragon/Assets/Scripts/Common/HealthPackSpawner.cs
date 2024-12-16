using UnityEngine;
using System.Collections.Generic;

public class HealthPackSpawner : MonoBehaviour
{
    // Singleton instance
    public static HealthPackSpawner Instance { get; private set; }

    public GameObject HealthPackPrefab;
    public List<Transform> SpawnPoints;
    private List<GameObject> activeHealthPacks = new List<GameObject>();
    private const int MaxHealthPacks = 2;

    private void Awake()
    {
        // Ensure that there's only one instance of HealthPackSpawner
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SpawnPoints.Count < 2)
        {
            Debug.LogError("You need at least 2 spawn points!");
            return;
        }

        // Initial spawning of health packs
        SpawnHealthPacks();
    }

    private void Update()
    {
        // Check and maintain only two health packs
        if (activeHealthPacks.Count < MaxHealthPacks)
        {
            SpawnHealthPacks();
        }
    }

    private void SpawnHealthPacks()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(SpawnPoints);
        while (activeHealthPacks.Count < MaxHealthPacks && availableSpawnPoints.Count > 0)
        {
            // Find two distant spawn points
            Transform spawnPoint1 = GetRandomSpawnPoint(availableSpawnPoints);
            availableSpawnPoints.Remove(spawnPoint1);
            Transform spawnPoint2 = GetFarthestSpawnPoint(spawnPoint1, availableSpawnPoints);

            // Spawn health packs
            if (spawnPoint1 != null)
            {
                activeHealthPacks.Add(Instantiate(HealthPackPrefab, spawnPoint1.position, Quaternion.identity));
            }
            if (spawnPoint2 != null)
            {
                activeHealthPacks.Add(Instantiate(HealthPackPrefab, spawnPoint2.position, Quaternion.identity));
            }
        }
    }

    private Transform GetRandomSpawnPoint(List<Transform> availableSpawnPoints)
    {
        if (availableSpawnPoints.Count == 0) return null;
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        return availableSpawnPoints[randomIndex];
    }

    private Transform GetFarthestSpawnPoint(Transform referencePoint, List<Transform> availableSpawnPoints)
    {
        if (availableSpawnPoints.Count == 0) return null;

        Transform farthestPoint = null;
        float maxDistance = float.MinValue;

        foreach (Transform point in availableSpawnPoints)
        {
            float distance = Vector3.Distance(referencePoint.position, point.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestPoint = point;
            }
        }

        availableSpawnPoints.Remove(farthestPoint);
        return farthestPoint;
    }

    public void RemoveHealthPack(GameObject healthPack)
    {
        if (activeHealthPacks.Contains(healthPack))
        {
            activeHealthPacks.Remove(healthPack);
            Destroy(healthPack);
        }
    }
}
