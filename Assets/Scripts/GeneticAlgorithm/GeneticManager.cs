using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    [SerializeField] GeneticPool geneticPool;
    [SerializeField] CarManager carManager;
    [SerializeField] TrackGrid trackGrid;
    [SerializeField] DeathLaser deathLaser;
    
    [Header("Controls")]
    public int initialPopulation;
    [Range(0.0f, 1.0f)]
    public float mutationRate;
    [Header("Crossover Controls")]
    public int bestAgentSelection;
    public int worstAgentSelection;
    public int numberCrossover;

    [Header("Public View")]
    public GameObject[] population;
    public int currentGeneration = 1;
    public float currentGenerationTime = 0f;
    public int collisedCars = 0;

    void Start()
    {
        CreatePopulation();
    }

    void FixedUpdate()
    {
        currentGenerationTime += Time.deltaTime;
        if(currentGenerationTime >= 15) deathLaser.AllowMoviment();
    }

    private void CreatePopulation()
    {
        population = new GameObject[initialPopulation];
        FillPopulationWithRandomValues(population, 0);
        StartPopulation();

    }

    private void FillPopulationWithRandomValues(GameObject[] population, int startingIndex)
    {
        while(startingIndex < initialPopulation)
        {
            population[startingIndex] = carManager.InstantiateRandomCar();
            NeuralNetwork network = carManager.GetNetwork(population[startingIndex]);
            network.RandomiseVariables();
            startingIndex++;
        }
    }

    private void StartPopulation()
    {
        foreach(GameObject car in population)
        {
            NeuralCar neuralCar = carManager.GetCar(car);
            neuralCar.AllowMoviment();
        }
    }

    public void Death(NeuralCar neuralCar)
    {
        CalculateFitness(neuralCar);
        collisedCars++;
        if(collisedCars == initialPopulation)
        {
            Repopulate();
        }
    }

    void CalculateFitness(NeuralCar neuralCar)
    {
        Vector3 carPosition = neuralCar.GetComponent<Transform>().position;
        Node node = trackGrid.NodeFromWorldPoint(carPosition);
        neuralCar.SetFitness(node.value);
    }

    void Repopulate()
    {
        geneticPool.Clear();
        SortPopulation();

        GameObject[] newPopulation = PickBestPopulation();
        Crossover(newPopulation);
        Mutate(newPopulation);
        RemoveCars(newPopulation);
        FillPopulationWithRandomValues(newPopulation, geneticPool.naturallySelected);
        ResetGeneration(newPopulation);
        StartPopulation();
    }

    void SortPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                NeuralCar car_one = carManager.GetCar(population[i]);
                NeuralCar car_two = carManager.GetCar(population[j]);
                if (car_one.fitness > car_two.fitness)
                {
                    GameObject temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }
    }

    GameObject[] PickBestPopulation()
    {
        GameObject[] newPopulation = new GameObject[initialPopulation];
        for (int i = 0; i < bestAgentSelection; i++)
        {
            int quantity = Mathf.RoundToInt(bestAgentSelection / (i + 1));

            geneticPool.Add(i, quantity);
            NeuralNetwork network = carManager.GetNetwork(population[i]);

            newPopulation[geneticPool.naturallySelected] = carManager.InstantiateCoypCar(network);
            geneticPool.naturallySelected++;
        }

        for(int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1;
            last -= i;

            int quantity = Mathf.RoundToInt(worstAgentSelection / (i + 1) / 2);
            geneticPool.Add(last, quantity);

        }

        return newPopulation;
    }

    void Crossover(GameObject[] newPopulation)
    {
        for(int i = 0; i < numberCrossover; i+=2)
        {
            (int AIndex, int BIndex) = geneticPool.GetIndex(i);

            NeuralNetwork parentOne = carManager.GetCopyNetwork(population[AIndex]);
            NeuralNetwork parentTwo = carManager.GetCopyNetwork(population[BIndex]);
            
            GameObject childOneGameObject = carManager.InstantiateRandomCar();
            NeuralNetwork childOne = carManager.GetNetwork(childOneGameObject);
            GameObject childTwoGameObject = carManager.InstantiateRandomCar();
            NeuralNetwork childTwo = carManager.GetNetwork(childOneGameObject);


            geneticPool.AssingParentsNetwork(childOne, childTwo, parentOne, parentTwo);
            newPopulation[geneticPool.naturallySelected] = childOneGameObject;
            geneticPool.naturallySelected++;
            newPopulation[geneticPool.naturallySelected] = childTwoGameObject;
            geneticPool.naturallySelected++;
        }
    }

    void Mutate(GameObject[] newPopulation)
    {
        for (int i = bestAgentSelection; i < geneticPool.naturallySelected; i++)
        {
            NeuralNetwork network = carManager.GetNetwork(newPopulation[i]);
            for(int c = 0; c < network.weights.Count; c++)
            {
                if(Random.Range(0f, 1f) < mutationRate)
                {
                    network.weights[c] = geneticPool.MutateMatrix(network.weights[c]);
                }
            }
        }
    }

    void RemoveCars(GameObject[] newPopulation)
    {
        GameObject[] carsToRemove = population.Except(newPopulation).ToArray();
        foreach(GameObject car in carsToRemove)
        {
            Destroy(car);
        }
    }

    void ResetGeneration(GameObject[] newPopulation)
    {
        population = newPopulation;
        collisedCars = 0;
        currentGeneration++;
        currentGenerationTime = 0f;
        deathLaser.Reset();
    }
}
