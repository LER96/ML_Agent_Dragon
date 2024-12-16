using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class EvolutionManager : MonoBehaviour
{
    public Transform target;
    private int populationSize = 100;
    private int generations = 50;
    private float mutationRate = 0.05f;
    private int numTopPerformers = 12;

    private GeneticAlgorithm geneticAlgorithm;
    private FitnessEvaluator fitnessEvaluator;

    void Start()
    {
        fitnessEvaluator = GetComponent<FitnessEvaluator>();
        // Updated to include only one hidden layer size (e.g., 7 for demonstration)
        geneticAlgorithm = new GeneticAlgorithm(populationSize, 11, 7, 4, mutationRate);
        RunEvolution();
    }

    private async void RunEvolution()
    {
        for (int generation = 0; generation < generations; generation++)
        {
            NeuralNetwork bestNetwork = null;
            float bestFitness = float.MinValue;
            List<float> fitnessScores = new List<float>();

            // Evaluate each network in the population
            foreach (var network in geneticAlgorithm.GetPopulation())
            {
                float fitness = await fitnessEvaluator.Evaluate(network);
                fitnessScores.Add(fitness);

                // Track the best network
                if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    bestNetwork = network;
                }
            }

            // Select top performers
            List<NeuralNetwork> topPerformers = SelectTopPerformers(geneticAlgorithm.GetPopulation(), fitnessScores, numTopPerformers);

            // Create a new population
            List<NeuralNetwork> newPopulation = new List<NeuralNetwork>();

            // Keep the top performers
            newPopulation.AddRange(topPerformers);

            // Fill the rest of the population with offspring
            while (newPopulation.Count < populationSize)
            {
                NeuralNetwork parent1 = topPerformers[Random.Range(0, topPerformers.Count)];
                NeuralNetwork parent2 = topPerformers[Random.Range(0, topPerformers.Count)];

                NeuralNetwork offspring = Crossover(parent1, parent2);
                Mutate(offspring, mutationRate);
                newPopulation.Add(offspring);
            }

            // Update the population
            geneticAlgorithm.GetPopulation().Clear();
            geneticAlgorithm.GetPopulation().AddRange(newPopulation);

            // Log the best fitness of the generation
            Debug.Log($"Generation {generation} complete. Best fitness: {bestFitness}");

            // Print the weights of the best network for this generation
            if (bestNetwork != null)
            {
                PrintNetworkWeights(bestNetwork);
                SaveBestNetworkWeights(bestNetwork, "Assets/BestNetworkWeights.json");
            }

            // Yield control back to Unity's main loop for a frame
            await Task.Yield();
        }
    }

    private void PrintNetworkWeights(NeuralNetwork network)
    {
        Debug.Log("Best Network Weights:");

        Debug.Log("Weights Input -> Hidden:");
        for (int i = 0; i < network.inputSize; i++)
        {
            string line = "";
            for (int j = 0; j < network.hiddenSize; j++)
            {
                line += network.weightsInputHidden[i, j].ToString("F4") + " ";
            }
            Debug.Log(line);
        }

        Debug.Log("Weights Hidden -> Output:");
        for (int i = 0; i < network.hiddenSize; i++)
        {
            string line = "";
            for (int j = 0; j < network.outputSize; j++)
            {
                line += network.weightsHiddenOutput[i, j].ToString("F4") + " ";
            }
            Debug.Log(line);
        }
    }

    private void SaveBestNetworkWeights(NeuralNetwork network, string filePath)
    {
        // Create an instance of NeuralNetworkData
        NeuralNetworkData data = new NeuralNetworkData(network.inputSize, network.hiddenSize, network.outputSize);

        // Copy the weights from the neural network into the data object
        for (int i = 0; i < network.inputSize; i++)
        {
            for (int j = 0; j < network.hiddenSize; j++)
            {
                data.weightsInputHidden[i][j] = network.weightsInputHidden[i, j];
            }
        }

        for (int i = 0; i < network.hiddenSize; i++)
        {
            for (int j = 0; j < network.outputSize; j++)
            {
                data.weightsHiddenOutput[i][j] = network.weightsHiddenOutput[i, j];
            }
        }

        // Serialize the data to JSON using Newtonsoft.Json
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);  // You can use Formatting.None for a compact file
        
        // Write the JSON string to the specified file
        File.WriteAllText(filePath, json);

        Debug.Log("Saved best network weights to " + filePath);
    }

    public List<NeuralNetwork> SelectTopPerformers(List<NeuralNetwork> population, List<float> fitnessScores, int numToSelect)
    {
        List<NeuralNetwork> selected = new List<NeuralNetwork>();

        // Sort networks by fitness
        List<int> sortedIndices = new List<int>();
        for (int i = 0; i < fitnessScores.Count; i++)
        {
            sortedIndices.Add(i);
        }
        sortedIndices.Sort((a, b) => fitnessScores[b].CompareTo(fitnessScores[a]));

        for (int i = 0; i < numToSelect; i++)
        {
            selected.Add(population[sortedIndices[i]]);
        }

        return selected;
    }

    public NeuralNetwork Crossover(NeuralNetwork parent1, NeuralNetwork parent2)
    {
        NeuralNetwork offspring = new NeuralNetwork(parent1.inputSize, parent1.hiddenSize, parent1.outputSize);

        for (int i = 0; i < parent1.weightsInputHidden.GetLength(0); i++)
        {
            for (int j = 0; j < parent1.weightsInputHidden.GetLength(1); j++)
            {
                offspring.weightsInputHidden[i, j] = (Random.value < 0.5f) ? parent1.weightsInputHidden[i, j] : parent2.weightsInputHidden[i, j];
            }
        }

        for (int i = 0; i < parent1.weightsHiddenOutput.GetLength(0); i++)
        {
            for (int j = 0; j < parent1.weightsHiddenOutput.GetLength(1); j++)
            {
                offspring.weightsHiddenOutput[i, j] = (Random.value < 0.5f) ? parent1.weightsHiddenOutput[i, j] : parent2.weightsHiddenOutput[i, j];
            }
        }

        return offspring;
    }

    public void Mutate(NeuralNetwork network, float mutationRate)
    {
        for (int i = 0; i < network.weightsInputHidden.GetLength(0); i++)
        {
            for (int j = 0; j < network.weightsInputHidden.GetLength(1); j++)
            {
                if (Random.value < mutationRate)
                {
                    network.weightsInputHidden[i, j] += Random.Range(-0.1f, 0.1f);
                }
            }
        }

        for (int i = 0; i < network.weightsHiddenOutput.GetLength(0); i++)
        {
            for (int j = 0; j < network.weightsHiddenOutput.GetLength(1); j++)
            {
                if (Random.value < mutationRate)
                {
                    network.weightsHiddenOutput[i, j] += Random.Range(-0.1f, 0.1f);
                }
            }
        }
    }
}
