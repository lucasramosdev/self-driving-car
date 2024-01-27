using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;
using Newtonsoft.Json;

public class SaveNetwork : MonoBehaviour
{
    [Header("Configurations")]
    public int maxLength = 150;
    string path = Application.dataPath + "\\Resources";

    void Start()
    {
        CheckBrainsFolder();
    }

    private void OnTriggerEnter(Collider collider)
    {
        GameObject car = GetCar(collider.gameObject);
        int filesCount = CheckBrainsFolderLength();
        if(car.tag == "NeuralCar" && filesCount < maxLength)
        {
            NeuralNetwork network = car.GetComponent<NeuralNetwork>();
            GenerateJson(network);
        }
    }

    GameObject GetCar(GameObject collider)
    {
        if(collider.gameObject.transform.parent.gameObject.transform.parent.gameObject)
        {
            GameObject parent = collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            return parent;
        }
        return null;
    }
    int CheckBrainsFolderLength()
    {
        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
        return dir.GetFiles().Length;
    }
    void CheckBrainsFolder()
    {
        if(!System.IO.Directory.Exists(path+"\\SavedBrains"))
        {
            System.IO.Directory.CreateDirectory(path + "\\SavedBrains");
        }
        path = path + "\\SavedBrains";
    }

    public void GenerateJson(NeuralNetwork network)
    {
        string ID = DateTime.Now.Ticks.ToString();
        Network network_object = new Network(network.weights, network.biases);
        string json = JsonConvert.SerializeObject(network_object);
        System.IO.File.WriteAllText(path + "\\" + ID + ".json", json);
    }

    
}


class Network
{
    public List<Matrix<float>> weights {get; set;}
    public List<Matrix<float>> biases {get; set;}
    public Network(List<Matrix<float>> weights, List<Matrix<float>> biases)
    {   
        this.weights = weights;
        this.biases = biases;
    }
}
