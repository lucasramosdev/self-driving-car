using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sensors : MonoBehaviour
{
    [HideInInspector]
    public float[] sensors;
    private int numberSensors;

    public void SetSensorsLength(int numberSensors)
    {
        this.numberSensors = numberSensors;
    }

    public float[] GetInput()
    {
        sensors = new float[numberSensors];
        int layerMask = LayerMask.GetMask("UnWalkable");
        float maxAngle = 180f;
        int angleStep = (int)(maxAngle / (numberSensors - 1));
        float initialAngle = -90F;
        float sensorLength = 30;
        Vector3 offset = new Vector3(0, 1, 0);
        for(int i = 0; i < sensors.Length; i++)
        {
            float angle = initialAngle + (i * angleStep);
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Ray r = new Ray(transform.position, direction);

            RaycastHit hit;
            if(Physics.Raycast(r, out hit, sensorLength, layerMask))
            {
                float sensor = sensorLength - hit.distance;
                sensors[i] = sensor;
                Debug.DrawRay(transform.position + offset, direction * hit.distance, Color.red);
            }
            else
            {
                sensors[i] = 0;
                Debug.DrawRay(transform.position + offset, direction * sensorLength, Color.green);
            }
        }
        sensors = NormalizeData(sensors);
        return sensors;
    }

    private float[] NormalizeData(float[] sensors)
    {
        float floor = .1f;

        float min = sensors.Min();
        float max = sensors.Max();

        float[] normalized = new float[sensors.Length];

        for(int i = 0; i < sensors.Length; i++)
        {
            float sensorNormalized = (sensors[i] - min) / (max - min);
            if(sensorNormalized < floor) sensorNormalized = 0f;
            normalized[i] = sensorNormalized;
        }

        return normalized;
    }
}
