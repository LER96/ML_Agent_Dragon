using UnityEngine;
using System.Threading.Tasks;

public class FitnessEvaluator : MonoBehaviour
{
    public Transform target;
    public GameObject dragonPrefab;
    public GameController gameController;
    public float targetHeight = 5.0f; // Adjust this to your desired target height
    public float groundThreshold = 1f; // Height considered as being "on the floor"
    public float heightRewardScale = 10.0f; // Scale for height reward
    public float floorPenalty = 10f; // Penalty for staying on the floor

    public float bonus = 1000;
    
    public async Task<float> Evaluate(NeuralNetwork network)
    {
        // Reset the dragon's position and state
        GameObject dragon = Instantiate(dragonPrefab, Vector3.up * 2, Quaternion.identity);
        DragonControllerNN dragonController = dragon.GetComponent<DragonControllerNN>();

        dragonController.target = target;
        dragonController.neuralNetwork = network;
        dragonController.fitness = this;

        dragonController.transform.position = Vector3.zero;
        dragonController.transform.rotation = Quaternion.identity;
        dragonController.rb.velocity = Vector3.zero;

        gameController.Reset();

        float fitness = 0f;
        float timeElapsed = 0f;
        float maxTime = 90f; // Maximum time allowed for the simulation

        //await Task.Delay(500);

        while (timeElapsed < maxTime)
        {
            // Gather inputs
            float[] inputs = dragonController.CollectInputs();

            // Get outputs from the neural network
            float[] outputs = network.FeedForward(inputs);

            // Apply outputs to control the dragon
            dragonController.ProcessNetworkOutputs(outputs);

            // Calculate distance to target
            float distanceToTarget = Vector3.Distance(dragon.transform.position, target.position);

            // Calculate height difference from the target height
            float heightDifference = Mathf.Abs(dragon.transform.position.y - target.position.y);

            // Reward for being closer to the target height
            float heightReward = heightRewardScale / (heightDifference + 1.0f);

            // Micro punishment for staying on the floor
            //float floorPenaltyApplied = dragon.transform.position.y < groundThreshold ? floorPenalty : 0f;

            // Combine fitness contributions

            fitness += (1.0f / (distanceToTarget + 1.0f)) + heightReward ;

            if(Mathf.Abs(Vector3.Distance(dragon.transform.position,target.position))<1){
                //fitness +=300;
            }

            if(bonus!=0){
                fitness +=bonus;
                bonus = 0;
            }
            
            timeElapsed += Time.deltaTime;

            // Yield control back to Unity's main loop
            await Task.Yield();
        }

        Destroy(dragon);
        return fitness;
    }
}
