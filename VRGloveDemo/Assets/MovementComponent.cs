using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class MovementComponent : MonoBehaviour {


    SerialPort serialPort;
    byte[] serialInformation; 
    void Start () {

        serialPort = new SerialPort("COM3", 9600);
        serialPort.Open();

        serialInformation = new byte[24]; // 6 floats 4 bytes each
    }
	
	// Update is called once per frame
	void Update () {
        string line;
        line = serialPort.ReadLine();
  
        string[] coordinateStrings = line.Split(' ');
        float[] coordinates = new float[6];
        for (int i = 0; i < coordinateStrings.Length; i++)
        {
            coordinates[i] = float.Parse(coordinateStrings[i]);
        }

        transform.Rotate(coordinates[0], coordinates[1], coordinates[2]);
       // transform.Translate(coordinates[3], coordinates[4], coordinates[5]);

    }
}
