using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class MovementComponent : MonoBehaviour {

    SerialPort serialPort;

    [SerializeField]
    string port = "COM3";

    private GloveController gloveController;
   
    float x = 0, y = 0, z = 0;
    string line; // a single line read from serial port
    string[] coordinateStrings = new string[3];



    void Start () {
        gloveController = FindObjectOfType<GloveController>();
        if(!gloveController)
        {
            Debug.LogError("No glove controller in scene!");
        }

        serialPort = new SerialPort(port, 38400);
        if (!serialPort.IsOpen)
        {
            Debug.LogWarning("Opening port!");
            serialPort.Open();
        }


    }
	
	// Update is called once per frame
	void Update () {
        line = serialPort.ReadLine();
     
        coordinateStrings = line.Split(' ');
        float[] coordinates = Array.ConvertAll(coordinateStrings, float.Parse);

        x = coordinates[0];
        y = coordinates[1];
        z = coordinates[2];
        
        transform.rotation = Quaternion.Euler(new Vector3(z, x, y));

        //Vector3 position = transform.position + new Vector3(coordinates[3], coordinates[4], coordinates[5]);
        //Quaternion rotation = new Quaternion()
        //transform.Rotate(coordinates[0], coordinates[1], coordinates[2]);
        // transform.Translate(coordinates[3], coordinates[4], coordinates[5]);
        Vector3 wristAccData = Vector3.zero;
        Vector3 gyroData = new Vector3(z, x, y);
        Vector3[] fingersAccData = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        gloveController.FeedMovementInfo(wristAccData, gyroData, fingersAccData);
    }
    

    private void OnDisable()
    {
        serialPort.Close();
    }
}
