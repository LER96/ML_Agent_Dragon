[System.Serializable]
public class NeuralNetworkData
{
    public float[][] weightsInputHidden;  // Weights between the input layer and the hidden layer
    public float[][] weightsHiddenOutput; // Weights between the hidden layer and the output layer

    public NeuralNetworkData(int inputSize, int hiddenSize, int outputSize)
    {
        // Initialize the weights for input to hidden layer
        weightsInputHidden = new float[inputSize][];
        for (int i = 0; i < inputSize; i++)
        {
            weightsInputHidden[i] = new float[hiddenSize];
        }

        // Initialize the weights for hidden to output layer
        weightsHiddenOutput = new float[hiddenSize][];
        for (int i = 0; i < hiddenSize; i++)
        {
            weightsHiddenOutput[i] = new float[outputSize];
        }
    }
}
