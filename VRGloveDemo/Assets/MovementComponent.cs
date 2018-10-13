using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

public class MovementComponent : MonoBehaviour {

    SerialPort serialPort;

    [SerializeField]
    string port = "COM3";

    [SerializeField]
    float multiplier = 10;

    private GloveController gloveController;

   
    float x_rot = 0, y_rot = 0, z_rot = 0;
    float x_acc = 0, y_acc = 0, z_acc = 0;

    //float x_speed_old = 0, y_speed_old = 0, z_speed_old = 0;
    float x_speed = 0, y_speed = 0, z_speed= 0;

   // float x_pos_old = 0, y_pos_old = 0, z_pos_old = 0;
    float x_pos = 0, y_pos = 0, z_pos= 0;



    int sensitivity = 16384;
    string line; // a single line read from serial port
    string[] coordinateStrings = new string[3];

    

    void Start () {
        gloveController = FindObjectOfType<GloveController>();
        if(!gloveController)
        {
            Debug.LogError("No glove controller in scene!");
        }

        serialPort = new SerialPort(port, 38400);
        serialPort.ReadTimeout = 1000 / 60; // 60 times a frame
        if (!serialPort.IsOpen)
        {
            Debug.LogWarning("Opening port!");
            serialPort.Open();
        }
        x_pos = transform.position.x;
        y_pos = transform.position.y;
        z_pos = transform.position.z;


    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            line = serialPort.ReadLine();
        }
        catch(System.Exception e)
        {
            Debug.LogError("Port Exception");
            return;
        }

        coordinateStrings = line.Split(' ');
        float[] coordinates = Array.ConvertAll(coordinateStrings, float.Parse);


        ChangePositionAndRotation(coordinates);

        float x = coordinates[0];
        float y = coordinates[1];
        float z = coordinates[2];
        
        //transform.rotation = Quaternion.Euler(new Vector3(z, x, y));

        //Vector3 position = transform.position + new Vector3(coordinates[3], coordinates[4], coordinates[5]);
        //Quaternion rotation = new Quaternion()
        //transform.Rotate(coordinates[0], coordinates[1], coordinates[2]);
        // transform.Translate(coordinates[3], coordinates[4], coordinates[5]);
        //Vector3 wristAccData = new Vector3(z, x, y);
        //Vector3 gyroData = new Vector3(z, x, y);
        //Vector3[] fingersAccData = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        //gloveController.FeedMovementInfo(wristAccData, gyroData, fingersAccData);
    }
    
    void ChangePositionAndRotation(float[] coordinates)
    {

        x_rot = coordinates[0];
        y_rot = coordinates[1];
        z_rot = coordinates[2];

        transform.rotation = Quaternion.Euler(new Vector3(z_rot, x_rot, y_rot));


        x_acc = coordinates[3] * multiplier; // / sensitivity ;
        y_acc = coordinates[4] * multiplier; // / sensitivity ;
        z_acc = coordinates[5] * multiplier; // / sensitivity;
        Debug.Log("x_acc: " + x_acc + "y_acc: " + y_acc + "z_acc: " + z_acc);
        x_speed = x_acc * Time.deltaTime;
        y_speed = y_acc * Time.deltaTime;
        z_speed = z_acc * Time.deltaTime;
        Debug.Log("x_speed_new: " + x_speed + "y_speed_new: " + y_speed + "z_speed_new: " + z_speed);
        x_pos += x_speed * Time.deltaTime;
        y_pos += y_speed * Time.deltaTime;
        z_pos += z_speed * Time.deltaTime;
        Debug.Log("x_pos_new: " + z_pos + "y_pos_new" + y_pos + "z_pos_new" + z_pos);
        transform.position = new Vector3(x_pos, z_pos, y_pos);

    }
    private void OnDisable()
    {
        serialPort.Close();
    }
}
