using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralCar : MonoBehaviour
{
    [SerializeField] CarController controller;
    [SerializeField] Sensors sensors;
    [Header("Configurations")]
    public bool testing;

    void Start()
    {
        sensors.SetSensorsLength(12);
    }
    void Update()
    {
        float speedSensor = controller.GetSpeed();
        sensors.GetInput();
    }
}
