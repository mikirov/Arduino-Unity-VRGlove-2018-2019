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
    [SerializeField] private String Ip = "192.168.1.6";

    [SerializeField] private int Port = 80;

    [SerializeField] private HandController handController;

//    public bool StateClient;

    public void Begin()
    {
        var thread = new Thread(() =>
        {
            var client = new TcpClient();
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
}