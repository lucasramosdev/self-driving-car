using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NeuralCar : MonoBehaviour
{
    [SerializeField] CarController controller;
    [SerializeField] Sensors sensorsController;
    [SerializeField] NeuralNetwork network;
    [SerializeField] Rigidbody rigidBody;
    GeneticManager manager;
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
        manager = GameObject.FindObjectOfType<GeneticManager>();
        if(!testing)
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void FixedUpdate()
    {
        if(testing)
        {
            GetSensors();
            GetNetworkResult();
            KeyboardMoviment();    
            return;
        }
        if(moviment)
        {
            GetSensors();
            GetNetworkResult();
            NeuralMoviment();   
        }

    }

    
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" && moviment)
        {
            Death();
        }
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
        controller.MoveCar(w, s, a, d);;
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

    void Death()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        moviment = false;
        manager.Death(this);
    }
}
