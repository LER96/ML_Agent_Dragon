using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Simulation : MonoBehaviour
{
    public int populationSize = 10;
    public int generations = 100;
    public float mutationRate = 0.01f;

    private List<float[]> population;
    private float[] fitnessScores;
    public StateMachine.StateMachine stateMachine;
    private bool roundEnd = false;

    void OnEnable()
    {
        EventManager.Instance.events.onRoundEnd.AddListener(() => roundEnd = true);
    }

    void OnDisable()
    {
        EventManager.Instance.events.onRoundEnd.RemoveListener(() => roundEnd = true);
    }

    void Start()
    {
        InitializePopulation();
        StartCoroutine(RunGA());
    }

    void InitializePopulation()
    {
        population = new List<float[]>();
        fitnessScores = new float[populationSize];
        
        for (int i = 0; i < populationSize; i++)
        {
            float[] individual = new float[7]; // Adjust based on the number of parameters
            
            individual[0] = Random.Range(10f, 50f); // MaxDistanceToMonster
            individual[1] = Random.Range(0f, 1f); // Weight for seeking health
            individual[2] = Random.Range(0f, 1f); // Weight for avoiding monster
            individual[3] = Random.Range(0f, 1f); // Weight for seeking coins
            individual[4] = Random.Range(3f, 15f);   // safeDistance
            individual[5] = Random.Range(1f, 6f);    // detectionRadius
            individual[6] = Random.Range(1f, 6f);    // fleeDistance

            population.Add(individual);
        }
    }

    IEnumerator RunGA()
    {
        for (int generation = 0; generation < generations; generation++)
        {
            List<float[]> newPopulation = new List<float[]>();

            // Evaluate each individual and store their fitness scores
            for (int i = 0; i < populationSize; i++)
            {
                float[] individual = population[i];
                stateMachine.ApplyGAParameters(individual);

                // Wait for 2 minutes or until roundEnd is true
                yield return StartCoroutine(WaitForEndOfRoundOrTime());

                float fitness = stateMachine.GetFitnessScore();
                fitnessScores[i] = fitness;  // Store the fitness score
                roundEnd = false;
            }

            // Sort the population based on fitness
            SortPopulationByFitness();

             PrintFitnessScores(generation);

            // Selection, crossover, and mutation
            while (newPopulation.Count < populationSize)
            {
                float[] parent1 = SelectIndividual(.3f);
                float[] parent2 = SelectIndividual(.3f);
                float[] offspring = Crossover(parent1, parent2);
                Mutate(offspring);
                newPopulation.Add(offspring);
            }

            // Replace the old population with the new one
            population = newPopulation;
        }

        // After the final generation, apply the best individual to your game
        ApplyBestIndividual();
    }

    IEnumerator WaitForEndOfRoundOrTime()
    {
        float waitTime = 120f; // 2 minutes in seconds
        float elapsedTime = 0f;

        // Reset roundEnd for the new round
        roundEnd = false;

        while (elapsedTime < waitTime && !roundEnd)
        {
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
    }

    void SortPopulationByFitness()
    {
        // Create a list of tuples (individual, fitness)
        List<(float[] individual, float fitness)> pairedList = new List<(float[], float)>();

        for (int i = 0; i < populationSize; i++)
        {
            pairedList.Add((population[i], fitnessScores[i]));
        }

        // Sort the list based on fitness (descending order)
        pairedList.Sort((x, y) => y.fitness.CompareTo(x.fitness));

        // Reassign the sorted individuals back to the population and fitnessScores
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = pairedList[i].individual;
            fitnessScores[i] = pairedList[i].fitness;
        }
    }

    float[] SelectIndividual(float topPercentage = 0.2f)
    {
        // Determine the number of top individuals to consider based on the given percentage
        int topCount = Mathf.CeilToInt(populationSize * topPercentage);

        // Randomly select an individual from the top-performing individuals
        int selectedIndex = Random.Range(0, topCount);

        return population[selectedIndex];
    }

    float[] SelectIndividual()
    {
        // Implement roulette wheel selection based on sorted fitnessScores
        float totalFitness = 0;

        for (int i = 0; i < populationSize; i++)
        {
            totalFitness += fitnessScores[i];
        }

        float randomPoint = Random.Range(0, totalFitness);
        float cumulativeFitness = 0;

        for (int i = 0; i < populationSize; i++)
        {
            cumulativeFitness += fitnessScores[i];
            if (cumulativeFitness >= randomPoint)
            {
                return population[i];
            }
        }

        // Fallback in case of rounding errors
        return population[populationSize - 1];
    }

    float[] Crossover(float[] parent1, float[] parent2)
    {
        float[] child = new float[parent1.Length];
        for (int i = 0; i < parent1.Length; i++)
        {
            child[i] = Random.value < 0.5f ? parent1[i] : parent2[i];
        }
        return child;
    }

    void Mutate(float[] individual)
    {
        for (int i = 0; i < individual.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                individual[i] += Random.Range(-1f, 1f); // Mutate by a small random amount
                // Ensure parameters stay within a valid range
                switch (i)
                {
                    case 0: // MaxDistanceToMonster
                        individual[i] = Mathf.Clamp(individual[i], 10f, 50f);
                        break;
                    case 1: // Weight for seeking health
                    case 2: // Weight for avoiding monster
                    case 3: // Weight for seeking coins
                        individual[i] = Mathf.Clamp(individual[i], 0f, 1f);
                        break;
                    case 4: // safeDistance
                        individual[i] = Mathf.Clamp(individual[i], 3f, 15f);
                        break;
                    case 5: // detectionRadius
                        individual[i] = Mathf.Clamp(individual[i], 1f, 6f);
                        break;
                    case 6: // fleeDistance
                        individual[i] = Mathf.Clamp(individual[i], 1f, 6f);
                        break;
                }
            }
        }
    }

    void ApplyBestIndividual()
    {
        float[] bestIndividual = population[0]; // Since the population is sorted, the first one is the best
        PrintBest(bestIndividual);
        stateMachine.ApplyGAParameters(bestIndividual);
    }
    private void PrintBest(float[] bestIndvidualParams){
        for(int i=0;i<bestIndvidualParams.Length;i++){
            Debug.Log($"{i} : {bestIndvidualParams[i]}");
        }
    }
    void PrintFitnessScores(int generation)
    {
        float cumulativeFitness = 0;
        float bestFitness = fitnessScores[0]; // The best fitness is at index 0 after sorting

        for (int i = 0; i < populationSize; i++)
        {
            cumulativeFitness += fitnessScores[i];
        }

        float averageFitness = cumulativeFitness / populationSize;

        Debug.Log($"Generation {generation + 1}:");
        Debug.Log($"Cumulative Fitness: {cumulativeFitness}");
        Debug.Log($"Average Fitness: {averageFitness}");
        Debug.Log($"Best Fitness: {bestFitness}");

        PrintBest(population[0]);
    }
}
