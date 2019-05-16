using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;

public class InputController : MonoBehaviour
{
    private HandController handController;
    private Thread thread;
    private TcpClient client;

    private StreamReader stream;
    private List<byte> buffer = new List<byte>();

    private void Start()
    {
        handController = FindObjectOfType<HandController>();
    }

    /*
    public void Connect(String Ip, int Port)
    {
        thread = new Thread(() =>
        {
            Debug.Log("Connect to network " + Ip + ":" + Port);
            client = new TcpClient();
            client.Connect(Ip, Port);

            var stream = new StreamReader(client.GetStream());

            List<byte> buffer = new List<byte>();


//            StateClient = client.Connected;
            //list of values read (0 - 1024 analog values)
            List<int> readings = new List<int>();

            while (client.Connected)
            {

                //Read the next byte
                int read = stream.Read();
                Debug.Log(read + " - " + (char) read);

                if (read == 32) // ascii code for space
                {
                    try
                    {
                        var str = Encoding.ASCII.GetString(buffer.ToArray());

                        int val = int.Parse(str);
                        readings.Add(val);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Misho e debil " + e);
                    }
                }

                //We split readings with a carriage return, so check for it
                if (read == 13) // carriage return code
                {
                    Debug.Log(readings.ToArray());

                    handController.SetReadings(readings);

                    Debug.Log(readings);
                    readings.Clear();
                    //Clear the buffer ready for another reading
                    buffer.Clear();
                }
                else
                    //if this was not the end of a reading, then just add this new byte to our buffer
                    buffer.Add((byte) read);
            }


            if (client.Connected)
            {
                client.Close();
            }
        });

        thread.Start();
    }
    */


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
            int[] values = valueString.Split(' ').Select(value => int.Parse(value)).ToArray();
            handController.SetReadings(values);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDestroy()
    {
        if (client.Connected)
        {
            client.Close();
        }
    }
}