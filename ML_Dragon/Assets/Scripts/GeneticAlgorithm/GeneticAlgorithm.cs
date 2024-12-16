using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    private List<NeuralNetwork> population;
    private int populationSize;
    private int inputSize;
    private int hiddenSize;
    private int outputSize;
    private float mutationRate;

    public GeneticAlgorithm(int populationSize, int inputSize, int hiddenSize, int outputSize, float mutationRate)
    {
        this.populationSize = populationSize;
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;
        this.mutationRate = mutationRate;

        population = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            population.Add(new NeuralNetwork(inputSize, hiddenSize, outputSize));
        }
    }

    public List<NeuralNetwork> GetPopulation()
    {
        return population;
    }
}
