using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Singleton instance
    public static GameController instance;

    public GameObject foodParticle; // Reference to the food particle prefab
    public Vector3 centerPoint; // The central point from which to spawn particles
    public float spawnRadius = 15f; // The fixed distance (radius) from the center point

    private GameObject currentFoodParticle;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this object
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set this instance and make sure it persists across scenes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        // Generate a random position in the upper hemisphere around the centerPoint
        Vector3 spawnPosition = GetRandomHemisphericalPoint(centerPoint, spawnRadius);

        // Instantiate the food particle at the generated position
        if (foodParticle != null)
        {
            //foodParticle = Instantiate(foodParticle, spawnPosition, Quaternion.identity);
            foodParticle.GetComponent<FoodParticle>().Setup(this);
        }
        Reset();
    }

    public void Reset()
    {
        // Reposition the existing food particle to a new random hemispherical position
        Vector3 randomPosition = GetRandomHemisphericalPoint(centerPoint, spawnRadius);

        if (foodParticle != null)
        {
            foodParticle.transform.position = randomPosition;
        }
    }

    Vector3 GetRandomHemisphericalPoint(Vector3 center, float radius)
    {
        // Generate a random point in the upper hemisphere using spherical coordinates
        float theta = Random.Range(0f, 2f * Mathf.PI); // Angle in the x-y plane
        float phi = Random.Range(0f, Mathf.PI / 2f); // Angle from the z-axis limited to the upper hemisphere

        // Convert spherical coordinates to Cartesian coordinates
        float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
        float z = radius * Mathf.Cos(phi);

        // Adjust y coordinate to ensure it is above the center point
        Vector3 randomPoint = center + new Vector3(x, y, z);

        // Ensure the y position is above the center point
        if (randomPoint.y <= center.y)
        {
            randomPoint.y = center.y + Mathf.Abs(randomPoint.y - center.y);
        }

        return randomPoint;
    }
}
