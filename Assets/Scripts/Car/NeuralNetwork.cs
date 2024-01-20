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
    List<Matrix<float>> weights = new List<Matrix<float>>();
    List<Matrix<float>> biases = new List<Matrix<float>>();

    List<float> neurons = new List<float>();

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

    public void RandomiseVariables()
    {
        Randomise(weights);
        Randomise(biases);
    }
    public void Randomise(List<Matrix<float>> matrix)
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
            neurons.Add(layer[0, i]);
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
