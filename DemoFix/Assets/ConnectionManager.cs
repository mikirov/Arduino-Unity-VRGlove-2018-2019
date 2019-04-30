using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    TMP_InputField ipInputField;

    [SerializeField]
    TMP_InputField portInputField;


    void Start()
    {
        ipInputField.text = PlayerPrefs.GetString("IP", "192.168.0.0");
        portInputField.text = "" + PlayerPrefs.GetInt("port", 80);
    }

    public void Connect()
    {
        string IP = ipInputField.text;
        int port = int.Parse(portInputField.text);
        PlayerPrefs.SetInt("port", port);
        PlayerPrefs.SetString("IP", IP);
        FindObjectOfType<InputController>().Connect(IP, port);
    }
}
