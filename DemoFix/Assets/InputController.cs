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
    private int[] trimmerValues;
    private Quaternion mpuQuaternion = Quaternion.identity;

    [SerializeField]
    private int trimmerCount = 2;

    [SerializeField]
    private float quaternionValueErrorDetectionLimit = 0.1f;

    [SerializeField]
    private float timeForForceRotationUpdate = 2;


    private float timeSinceLastUpdate = 0;

    private void Start()
    {
        trimmerValues = new int[trimmerCount];
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
                Debug.LogError("Wrong number of trimmer values read " + trimmerValues.Count);
            }
            else
            {
                string vals = "";
                foreach(float f in trimmerValues)
                {
                    vals += f.ToString() + " ";
                }
                Debug.Log(vals);

                this.trimmerValues = trimmerValues.ToArray();
            }
            
            
            if(mpuQuaternion == Quaternion.identity || 
                timeSinceLastUpdate >= timeForForceRotationUpdate || 
                quaternionValuesAreCorrect(quaternionValues))
            {
                mpuQuaternion = new Quaternion(
                    -quaternionValues[1],
                    -quaternionValues[3],
                    -quaternionValues[2],
                    quaternionValues[0]
                );
                timeSinceLastUpdate = 0;
            }
            else
            {
                Debug.LogError("MPU Values invalid this frame!");
            }


            timeSinceLastUpdate += Time.deltaTime;
            stream.DiscardBufferedData();
            yield return new WaitForEndOfFrame();
        }
    }

    private bool quaternionValuesAreCorrect(List<float> quaternionValues)
    {
        if(quaternionValues.Count != 4)
        {
            return false;
        }

        List<float> lastQuaternionValues = new List<float>
            { mpuQuaternion.w, mpuQuaternion.x, mpuQuaternion.z, mpuQuaternion.y};


        for(int i = 0; i < quaternionValues.Count; i++)
        {
            float quatDifference = Mathf.Abs(lastQuaternionValues[i]) - Mathf.Abs(quaternionValues[i]);
            if ( Mathf.Abs(quaternionValues[i]) <= float.Epsilon || 
                !quaternionValueIsInAcceptableRange(quaternionValues[i]) ||
                quatDifference > quaternionValueErrorDetectionLimit)
            {
                Debug.Log("Quat difference: " + quatDifference);
                return false;
            }
        }

        return true;
    }

    private bool quaternionValueIsInAcceptableRange(float val)
    {
        return val < 1 && val > -1;
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
        return trimmerValues.ToList();
    }

    public override Quaternion GetMPUValues()
    {
        return mpuQuaternion;
    }
}