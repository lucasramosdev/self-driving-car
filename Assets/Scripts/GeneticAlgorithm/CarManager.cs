using UnityEngine;

public class CarManager : MonoBehaviour
{
    private GameObject carPrefab;
    public Transform spawnPoint;

    [Header("Network Controls")]
    public int spaceSensors;
    public int controllerSensors;
    public int hiddenLayers;
    public int hiddenNeurons;
    public int numberOutput;

    private void Awake()
    {
        carPrefab = Resources.Load<GameObject>("prefabs/Prometheus");
    }
    public GameObject InstantiateRandomCar()
    {
        GameObject car = InstantiateCar();
        NeuralNetwork network = GetNetwork(car);
        network.Initialise(hiddenLayers, hiddenNeurons, spaceSensors + controllerSensors, numberOutput);
        return car;
    }

    public GameObject InstantiateCoypCar(NeuralNetwork parent)
    {
        GameObject car = InstantiateCar();
        NeuralNetwork network = GetNetwork(car);
        network.InitialiseCopy(parent, hiddenLayers, hiddenNeurons);
        return car;
    }

    public GameObject InstantiateCar()
    {
        GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        
        Sensors sensors = car.GetComponent<Sensors>();
        sensors.SetSensorsLength(spaceSensors);

        return car;
    }
    
    public NeuralCar GetCar(GameObject carGameObject)
    {
        NeuralCar car = carGameObject.GetComponent<NeuralCar>();
        return car;
    }
    public NeuralNetwork GetNetwork(GameObject carGameOject)
    {
        NeuralNetwork network = carGameOject.GetComponent<NeuralNetwork>();
        return network;
    }
    public NeuralNetwork GetCopyNetwork(GameObject carGameOject)
    {
        NeuralNetwork network = carGameOject.GetComponent<NeuralNetwork>();
        return network.CopyNetwork(hiddenLayers, hiddenNeurons);
    }
}
