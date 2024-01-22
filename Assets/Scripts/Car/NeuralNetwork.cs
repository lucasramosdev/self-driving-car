using System;
using UnityEngine;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

using Random = UnityEngine.Random;

public class NeuralNetwork : MonoBehaviour
{
    int[] outputLayer;
    Matrix<float> inputLayer;
    List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    List<float> neurons = new List<float>();
    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<Matrix<float>> biases = new List<Matrix<float>>();

    public void Initialise(int hiddenLayersCount, int hiddenNeuronsCount, int sensorsCount, int outputNeuronsCount)
    {
        for(int i = 0; i < hiddenLayersCount; i++)
        {
            Matrix<float> layer = Matrix<float>.Build.Dense(1, hiddenNeuronsCount);
            hiddenLayers.Add(layer);
            Matrix<float> bias = Matrix<float>.Build.Dense(1, hiddenNeuronsCount);
            biases.Add(bias);
            if(i == 0)
            {
                Matrix<float> inputToHidden = Matrix<float>.Build.Dense(sensorsCount, hiddenNeuronsCount);
                weights.Add(inputToHidden);
                continue;
            }

            Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronsCount, hiddenNeuronsCount);
            weights.Add(hiddenToHidden);
        }
        Matrix<float> outputBias = Matrix<float>.Build.Dense(1, outputNeuronsCount);
        biases.Add(outputBias);
        Matrix<float> weightBias = Matrix<float>.Build.Dense(hiddenNeuronsCount, outputNeuronsCount);
        weights.Add(weightBias);
    }

    public void InitialiseCopy(NeuralNetwork parent, int layers, int neurons)
    {
        List<Matrix<float>> newWeights = new List<Matrix<float>>();
        List<Matrix<float>> newBiases = new List<Matrix<float>>();

        for(int i = 0; i < parent.weights.Count; i++)
        {
            Matrix<float> currentWeight = Matrix<float>.Build.Dense(parent.weights[i].RowCount, parent.weights[i].ColumnCount);
            for(int x = 0; x < currentWeight.RowCount; x++)
            {
                for(int y = 0; y < currentWeight.ColumnCount; y++)
                {
                    currentWeight[x, y] = parent.weights[i][x, y];
                }
            }
            newWeights.Add(currentWeight);
        }

        for(int i = 0; i < parent.biases.Count; i++)
        {
            Matrix<float> currentBias = Matrix<float>.Build.Dense(parent.biases[i].RowCount, parent.biases[i].ColumnCount);
            for(int x = 0; x < currentBias.RowCount; x++)
            {
                for(int y = 0; y < currentBias.ColumnCount; y++)
                {
                    currentBias[x, y] = parent.biases[i][x, y];
                }
            }
            newBiases.Add(currentBias);
        }

        biases = newBiases;
        weights = newWeights;
        InitialiseHidden(layers, neurons);
    }

    public NeuralNetwork CopyNetwork(int hiddenLayers, int hiddenNeurons)
    {
        NeuralNetwork copyNetwork = new NeuralNetwork();
        List<Matrix<float>> newWeights = new List<Matrix<float>>();
        List<Matrix<float>> newBiases = new List<Matrix<float>>();

        for(int i = 0; i < weights.Count; i++)
        {
            Matrix<float> currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);
            for(int x = 0; x < currentWeight.RowCount; x++)
            {
                for(int y = 0; y < currentWeight.ColumnCount; y++)
                {
                    currentWeight[x, y] = weights[i][x, y];
                }
            }
            newWeights.Add(currentWeight);
        }

        for(int i = 0; i < biases.Count; i++)
        {
            Matrix<float> currentBias = Matrix<float>.Build.Dense(biases[i].RowCount, biases[i].ColumnCount);
            for(int x = 0; x < currentBias.RowCount; x++)
            {
                for(int y = 0; y < currentBias.ColumnCount; y++)
                {
                    currentBias[x, y] = biases[i][x, y];
                }
            }
            newBiases.Add(currentBias);
        }

        copyNetwork.biases = newBiases;
        copyNetwork.weights = newWeights;
        copyNetwork.InitialiseHidden(hiddenLayers, hiddenNeurons);
        return copyNetwork;
    }

    private void InitialiseHidden(int hiddenLayerCount, int hiddenNeuronCount)
    {
        hiddenLayers.Clear();

        for (int i = 0; i < hiddenLayerCount; i ++)
        {
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(newHiddenLayer);
        }
    }

    public void RandomiseVariables()
    {
        Randomise(weights);
        Randomise(biases);
    }
    private void Randomise(List<Matrix<float>> matrix)
    {

        for (int i = 0; i < matrix.Count; i++)
        {

            for (int x = 0; x < matrix[i].RowCount; x++)
            {

                for (int y = 0; y < matrix[i].ColumnCount; y++)
                {

                    matrix[i][x, y] = Random.Range(-1f, 1f);

                }

            }

        }
    }

    public List<float> GetNeurons()
    {
        return neurons;
    }


    public (int, int, int, int) RunNetwork(float[] sensors)
    {
        neurons.Clear();
        MakeInputLayer(sensors);
        for(int i = 0; i < hiddenLayers.Count; i++) MakeHiddenLayer(i);
        int[] outputLayer = Array.ConvertAll(MakeOutputLayer(), output => (int)output);
        return (
            outputLayer[0],
            outputLayer[1],
            outputLayer[2],
            outputLayer[3]
        );
    }

    private void MakeInputLayer(float[] sensors)
    {
        inputLayer = Matrix<float>.Build.Dense(1, sensors.Length);
        for(int i = 0; i < sensors.Length; i++)
        {
            inputLayer[0, i] = sensors[i];
            neurons.Add(sensors[i]);
        }
    }
    private void MakeHiddenLayer(int index)
    {
        if(index == 0)
        {
            hiddenLayers[index] = StepFunction(inputLayer * weights[index] + biases[index]);
            return;
        }
        hiddenLayers[index] = StepFunction(hiddenLayers[index - 1] * weights[index] + biases[index]);
    }

    private float[] MakeOutputLayer()
    {
        Matrix<float> multiplied_output_weights = hiddenLayers[hiddenLayers.Count-1] * weights[weights.Count-1];
        Matrix<float> added_output_bias = multiplied_output_weights + biases[biases.Count-1];
        Matrix<float> layer = StepFunction(added_output_bias); 
        float[] output = new float[layer.ColumnCount];
        for (int i = 0; i < layer.ColumnCount; i++)
        {
            output[i] = layer[0,i];
        }
        return output;
    }

    private  Matrix<float> StepFunction(Matrix<float> matrix)
    {
        int rows = matrix.RowCount;
        int columns = matrix.ColumnCount;

        Matrix<float> resultMatrix = Matrix<float>.Build.Dense(rows, columns);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                resultMatrix[i,j] = (matrix[i,j] > 0.0f) ? 1.0f : 0.0f;
                neurons.Add(resultMatrix[i,j]);
            }
        }

        return resultMatrix;
    }
}
