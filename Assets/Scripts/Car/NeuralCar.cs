using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NeuralCar : MonoBehaviour
{
    [SerializeField] CarController controller;
    [SerializeField] Sensors sensorsController;
    [SerializeField] NeuralNetwork network;
    [SerializeField] Rigidbody rigidBody;
    bool moviment = false;
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

    void Awake()
    {
        if(!testing)
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    
    void FixedUpdate()
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

    public void AllowMoviment()
    {
        if(moviment == false)
        {
            rigidBody.constraints = RigidbodyConstraints.None;
            moviment = true;
        }
    }

    public void SetFitness(int value)
    {
        fitness = value;
    }

    void NeuralMoviment()
    {
        controller.MoveCar(w, s, a, d);
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
