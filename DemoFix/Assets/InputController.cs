using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

public class InputController : MonoBehaviour
{
    private HandController handController;
    private Thread thread;
    private TcpClient client;

    private void Start()
    {
        handController = FindObjectOfType<HandController>();
    }
    
    public void Connect(String Ip, int Port)
    {
        thread = new Thread(() =>
        {
            Debug.Log("Connect to network");
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


                if (read == 32) // ascii code for space
                {
                    var str = Encoding.ASCII.GetString(buffer.ToArray());

                    var val = int.Parse(str);
                    readings.Add(val);
                }

                //We split readings with a carriage return, so check for it
                if (read == 13) // carriage return code
                {
                    Debug.Log(readings);

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
        });

        thread.Start();
    }

    private void OnDestroy()
    {
        thread.Abort();
        if (client.Connected)
        {
            client.Close();
        }
    }
}