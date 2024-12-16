using UnityEngine;
using System;
using Newtonsoft.Json;

public class NeuralNetwork
{
    public int inputSize;
    public int hiddenSize;  // Size of the single hidden layer
    public int outputSize;

    public float[] inputs;
    public float[] hiddenLayer;
    public float[] outputs;

    public float[,] weightsInputHidden;
    public float[,] weightsHiddenOutput;

    public System.Random random;

    public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        inputs = new float[inputSize];
        hiddenLayer = new float[hiddenSize];
        outputs = new float[outputSize];

        weightsInputHidden = new float[inputSize, hiddenSize];
        weightsHiddenOutput = new float[hiddenSize, outputSize];

        random = new System.Random();

        InitializeWeights();

        LoadNetworkWeights("Assets/Weights.json");
        
        //PerturbWeights();
    }

    private void InitializeWeights()
    {
        // Initialize weights between input and hidden layer
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                weightsInputHidden[i, j] = (float)random.NextDouble() * 2 - 1; // Random weights between -1 and 1
            }
        }

        // Initialize weights between hidden layer and output layer
        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weightsHiddenOutput[i, j] = (float)random.NextDouble() * 2 - 1; // Random weights between -1 and 1
            }
        }
    }

public void PerturbWeights()
{
    // Adjust weights between input and hidden layer
    for (int i = 0; i < inputSize; i++)
    {
        for (int j = 0; j < hiddenSize; j++)
        {
            // Add a random value between -0.05 and 0.05 to each weight
            weightsInputHidden[i, j] += (float)(random.NextDouble() * 0.1 - 0.05);
        }
    }

    // Adjust weights between hidden layer and output layer
    for (int i = 0; i < hiddenSize; i++)
    {
        for (int j = 0; j < outputSize; j++)
        {
            // Add a random value between -0.05 and 0.05 to each weight
            weightsHiddenOutput[i, j] += (float)(random.NextDouble() * 0.1 - 0.05);
        }
    }

    Debug.Log("Perturbed network weights by random values between -0.05 and 0.05");
}



    public float[] FeedForward(float[] input)
    {
        // Feed inputs through the hidden layer
        for (int i = 0; i < hiddenSize; i++)
        {
            hiddenLayer[i] = 0;
            for (int j = 0; j < inputSize; j++)
            {
                hiddenLayer[i] += input[j] * weightsInputHidden[j, i];
            }
            hiddenLayer[i] = Tanh(hiddenLayer[i]); // Activation function
        }

        // Feed hidden layer through to the output layer
        for (int i = 0; i < outputSize; i++)
        {
            outputs[i] = 0;
            for (int j = 0; j < hiddenSize; j++)
            {
                outputs[i] += hiddenLayer[j] * weightsHiddenOutput[j, i];
            }
            outputs[i] = Tanh(outputs[i]); // Activation function
        }

        return outputs;
    }

    private float Tanh(float value)
    {
        return (float)Math.Tanh(value);
    }

   public void LoadNetworkWeights(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("Weights file not found at " + filePath);
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);
        NeuralNetworkData data = JsonConvert.DeserializeObject<NeuralNetworkData>(json);

        // Ensure the sizes match
        if (data.weightsInputHidden.Length != inputSize || 
            data.weightsHiddenOutput.Length != hiddenSize || 
            data.weightsInputHidden[0].Length != hiddenSize || 
            data.weightsHiddenOutput[0].Length != outputSize)
        {
            Debug.LogError("Weights data size mismatch.");
            return;
        }

        // Initialize weights if not already initialized
        if (weightsInputHidden == null)
        {
            weightsInputHidden = new float[inputSize, hiddenSize];
        }
        if (weightsHiddenOutput == null)
        {
            weightsHiddenOutput = new float[hiddenSize, outputSize];
        }

        // Load weights
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                weightsInputHidden[i, j] = data.weightsInputHidden[i][j];
            }
        }

        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                weightsHiddenOutput[i, j] = data.weightsHiddenOutput[i][j];
            }
        }

        Debug.Log("Loaded network weights from " + filePath);
    }

}
