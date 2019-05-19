using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;

public class InputController : BaseInputController
{
    private HandController handController;
    private TcpClient client;

    private StreamReader stream;
    private List<int> values = new List<int>();

    private void Start()
    {
        handController = FindObjectOfType<HandController>();
    }


    public void ConnectClient(String Ip, int Port)
    {
        Debug.Log("Connect to network " + Ip + ":" + Port);
        client = new TcpClient();
        client.Connect(Ip, Port);

        stream = new StreamReader(client.GetStream());
        StartCoroutine(Client());
    }


    private IEnumerator Client()
    {
        while (true)
        {
            if (!client.Connected)
            {
                Debug.Log("Disconnected");
                break;
            }
            string valueString = stream.ReadLine();
            values = valueString.Split(' ').Select(value => int.Parse(value)).ToList();
            stream.DiscardBufferedData();
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDestroy()
    {
        if (client != null && client.Connected)
        {
            client.Close();
        }
    }

    public override List<int> GetValues()
    {
        return values;
    }
}