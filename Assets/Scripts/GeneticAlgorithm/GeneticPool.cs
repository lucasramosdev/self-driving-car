using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class GeneticPool : MonoBehaviour
{
    private List<int> genePool = new List<int>();
    public int naturallySelected;

    public void Clear()
    {
        genePool.Clear();
        naturallySelected = 0;
    }

    public int GetLength()
    {
        return genePool.Count;
    }

    public void Add(int index, int quantity)
    {
        for (int c = 0; c < quantity; c++)
        {
            genePool.Add(index);
        }
    }

    public (int, int) GetIndex(int index)
    {
        int AIndex = index;
        int BIndex = index + 1;
        for(int i = 0; i < 100; i++)
        {
            AIndex = genePool[Random.Range(0, genePool.Count)];
            BIndex = genePool[Random.Range(0, genePool.Count)];
            if(AIndex != BIndex) break;
        }

        return (AIndex, BIndex);
    }

    public void AssingParentsNetwork(NeuralNetwork childOne, NeuralNetwork childTwo, NeuralNetwork parentOne, NeuralNetwork parentTwo)
    {
        for (int w = 0; w < childOne.weights.Count; w++)
        {
            if(Random.Range(0f, 1f) < .5f)
            {
                childOne.weights[w] = parentOne.weights[w];
                childTwo.weights[w] = parentTwo.weights[w];
            }
            else{
                childOne.weights[w] = parentTwo.weights[w];
                childTwo.weights[w] = parentOne.weights[w];
            }
        }

        for (int b = 0; b < childOne.biases.Count; b++)
        {
            if(Random.Range(0f, 1f) < .5f)
            {
                childOne.biases[b] = parentOne.biases[b];
                childTwo.biases[b] = parentTwo.biases[b];
            }
            else{
                childOne.biases[b] = parentTwo.biases[b];
                childTwo.biases[b] = parentOne.biases[b];
            }
        }
    }

    public Matrix<float> MutateMatrix(Matrix<float> matrix)
    {   
        int maxRandomPoints = matrix.RowCount * matrix.ColumnCount / 7;
        int randomPoints = Random.Range(0, maxRandomPoints);
        float min = -1f;
        float max = 1f;

        Matrix<float> newMatrix = matrix;

        for(int i = 0; i < randomPoints; i++)
        {
            int randomRow = Random.Range(0, matrix.ColumnCount);
            int randomColumn = Random.Range(0, matrix.ColumnCount);

            newMatrix[randomRow, randomColumn] = Mathf.Clamp(newMatrix[randomRow, randomColumn] + Random.Range(min, max), min, max);
        }
        return newMatrix;
    }
}
