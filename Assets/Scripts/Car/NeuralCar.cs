using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NeuralCar : MonoBehaviour
{
    [SerializeField] CarController controller;
    [SerializeField] Sensors sensorsController;
    public float[] sensors;
    [Header("Configurations")]
    public bool testing;

    [Header("Results")]
    public int w;
    public int s;
    public int a;
    public int d;

    void Start()
    {
        sensorsController.SetSensorsLength(12);
    }
    void Update()
    {
        GetSensors();

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
        w = Input.GetKey(KeyCode.W) == true ? 1 : 0;
        s = Input.GetKey(KeyCode.S) == true ? 1 : 0;
        a = Input.GetKey(KeyCode.A) == true ? 1 : 0;
        d = Input.GetKey(KeyCode.D) == true ? 1 : 0;
        controller.MoveCar(w, s, a, d);
    }

    void GetSensors()
    {
        float speedSensor = controller.GetSpeed();
        float [] spacialSensors = sensorsController.GetInput();
        sensors = new float[spacialSensors.Length + 1];
        spacialSensors.CopyTo(sensors, 0);
        sensors[sensors.Length - 1] = speedSensor;
    }
}
