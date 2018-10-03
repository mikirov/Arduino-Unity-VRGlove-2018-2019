using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class MovementComponent : MonoBehaviour {

    SerialPort serialPort;
    byte[] serialInformation;
    float multiplier = 0.25f;
    float x = 0, y = 0, z = 0;

    void Start () {

        serialPort = new SerialPort("COM5", 9600);
        if (!serialPort.IsOpen)
        {
            Debug.LogWarning("Opening port!");
            serialPort.Open();
        }

        serialInformation = new byte[24]; // 6 floats 4 bytes each

    }
	
	// Update is called once per frame
	void Update () {
        string line;

        line = serialPort.ReadLine();

        

        // Debug.Log(line);

        string[] coordinateStrings = line.Split(' ');
        float[] coordinates = new float[coordinateStrings.Length];
        Debug.Log(coordinateStrings.Length);
        for (int i = 0; i < coordinateStrings.Length; i++)
        {
            coordinates[i] = float.Parse(coordinateStrings[i], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);

        }
        if (coordinates[0] != 0 && coordinates[1] != 0 && coordinates[2] != 0)
        {
            x = coordinates[0];
            y = coordinates[1];
            z = coordinates[2];
        }
        transform.rotation = Quaternion.Euler(new Vector3(z, x, y));
     
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
