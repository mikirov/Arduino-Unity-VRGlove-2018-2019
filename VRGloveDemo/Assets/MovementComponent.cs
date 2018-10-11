﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class MovementComponent : MonoBehaviour {

    SerialPort serialPort;

    [SerializeField]
    string port = "COM3";
    [SerializeField]
    int multiplier = 10;
   
    float x_rot = 0, y_rot = 0, z_rot = 0;
    float x_pos = 0, y_pos = 0, z_pos = 0;
    int sensitivity = 16384;
    string line; // a single line read from serial port
    string[] coordinateStrings = new string[3];


    void Start () {

        serialPort = new SerialPort(port, 38400);
        if (!serialPort.IsOpen)
        {
            Debug.LogWarning("Opening port!");
            serialPort.Open();
        }


    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            line = serialPort.ReadLine();

            coordinateStrings = line.Split(' ');
            float[] coordinates = Array.ConvertAll(coordinateStrings, float.Parse);

            x_rot = coordinates[0];
            y_rot = coordinates[1];
            z_rot = coordinates[2];

            transform.rotation = Quaternion.Euler(new Vector3(z_rot, x_rot, y_rot));

            x_pos = coordinates[3] / sensitivity * multiplier;
            y_pos = coordinates[4] / sensitivity * multiplier;
            z_pos = coordinates[5] / sensitivity * multiplier;

            transform.position = new Vector3(x_pos, y_pos, z_pos);
        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }
        //Vector3 position = transform.position + new Vector3(coordinates[3], coordinates[4], coordinates[5]);
        //Quaternion rotation = new Quaternion()
        //transform.Rotate(coordinates[0], coordinates[1], coordinates[2]);
        // transform.Translate(coordinates[3], coordinates[4], coordinates[5]);

    }
    

    private void OnDisable()
    {
        serialPort.Close();
    }
}
