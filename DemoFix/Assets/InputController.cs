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
    private List<int> trimmerValues = new List<int>();
    private Quaternion mpuQuaternion = Quaternion.identity;

    [SerializeField]
    private int trimmerCount = 2;

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

            Debug.Log(valueString);
            List<float> quaternionValues = new List<float>();
            List<int> trimmerValues = new List<int>();

            foreach(String val in valueString.Split(' '))
            {
                int trimmerVal;
                float quaternionVal;

                if(int.TryParse(val, out trimmerVal))
                {
                    trimmerValues.Add(trimmerVal);
                }
                else if(float.TryParse(val, out quaternionVal))
                {
                    quaternionValues.Add(quaternionVal);
                }
            }

            if (trimmerValues.Count != trimmerCount)
            {
                Debug.LogError("Wrong number of trimmer values read");
            }
            else
            {
                this.trimmerValues = trimmerValues;
            }
            
            if(quaternionValues.Count == 4)
            {
                mpuQuaternion = new Quaternion(
                    quaternionValues[0],
                    quaternionValues[1],
                    quaternionValues[2],
                    quaternionValues[3]
                );
            }
            else
            {
                Debug.LogWarning("MPU Values invalid this frame: ");
            }
            
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
        return trimmerValues;
    }

    public override Quaternion GetMPUValues()
    {
        return mpuQuaternion;
    }
}