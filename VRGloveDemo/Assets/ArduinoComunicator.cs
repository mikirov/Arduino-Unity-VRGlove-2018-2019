using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class ArduinoComunicator : MonoBehaviour {

    [SerializeField]
    private int gyroCount = 1;


    SerialPort sp;

    void Start()
    {
        sp = new SerialPort("COM5", 9600);
        sp.Open();
        sp.ReadTimeout = 1;
    }

    void Update()
    {
        try
        {
            Debug.Log(sp.ReadLine());
        }
        catch (System.Exception)
        {
            Debug.LogError("An error occured!");
        }
    }
}
