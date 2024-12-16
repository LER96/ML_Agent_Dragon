using UnityEngine;

public class Coin : SpawnedObject
{
    // Rotation speed (degrees per second)
    public float rotationSpeed = 90f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the coin around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    protected override void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
           other.GetComponent<StateMachine.StateMachine>().AddCoinReward();
           base.OnTriggerEnter(other);
        }
        
    }
}
