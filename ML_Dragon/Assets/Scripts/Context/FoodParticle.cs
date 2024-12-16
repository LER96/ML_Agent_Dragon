using UnityEngine;

public class FoodParticle : MonoBehaviour
{
    private GameController controller;
    public void Setup(GameController controller)
    {
        this.controller = controller;
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Dragon")){
           // controller.Reset();
        }
    }
}
