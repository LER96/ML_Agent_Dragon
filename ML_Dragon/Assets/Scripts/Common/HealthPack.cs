using UnityEngine;

public class HealthPack : SpawnedObject
{
    private float healthAmount = 50;
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           StateMachine.StateMachine stateMachine = other.GetComponent<StateMachine.StateMachine>(); 
           if(stateMachine!=null){
            stateMachine.AddHealth(healthAmount);
           }
           base.OnTriggerEnter(other);
        }
         
    }
}
