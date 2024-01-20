using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NeuralCar : MonoBehaviour
{
    [SerializeField] CarController controller;
    [SerializeField] Sensors sensorsController;
    [SerializeField] NeuralNetwork network;
    public float[] sensors;
    [Header("Configurations")]
    public bool testing;

    [Header("Neural Network")]
    public float fitness;
    public List<float> neurons = new List<float>();

    [Header("Results")]
    public int w;
    public int s;
    public int a;
    public int d;

    void Start()
    {
        sensorsController.SetSensorsLength(12);
        network.Initialise(1, 4, 13, 4);
        network.RandomiseVariables();
    }
    void Update()
    {
        GetSensors();
        GetNetworkResult();
        if(testing)
        {
            KeyboardMoviment();    
            return;
        }

        NeuralMoviment();

    }

    void NeuralMoviment()
    {

    }

    void KeyboardMoviment()
    {
        int humanW = Input.GetKey(KeyCode.W) == true ? 1 : 0;
        int humanS = Input.GetKey(KeyCode.S) == true ? 1 : 0;
        int humanA = Input.GetKey(KeyCode.A) == true ? 1 : 0;
        int humanD = Input.GetKey(KeyCode.D) == true ? 1 : 0;
        controller.MoveCar(humanW, humanS, humanA, humanD);
    }

    void GetSensors()
    {
        float speedSensor = controller.GetSpeed();
        float [] spacialSensors = sensorsController.GetInput();
        sensors = new float[spacialSensors.Length + 1];
        spacialSensors.CopyTo(sensors, 0);
        sensors[sensors.Length - 1] = speedSensor;
    }

    void GetNetworkResult()
    {
        neurons = network.GetNeurons();
        (w, s, a, d) = network.RunNetwork(sensors);
    }
}
