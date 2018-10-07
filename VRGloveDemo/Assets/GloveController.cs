using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GloveController : MonoBehaviour {

    [SerializeField]
    private GameObject[] fingers = new GameObject[5];

    [SerializeField]
    private bool ApplyShakynessFilter = true;

    public void FeedMovementInfo(Vector3 wristAccData, Vector3 wristGyroData, Vector3[] fingersAccData)
    {
        transform.position += wristAccData;
        transform.eulerAngles = wristGyroData;
        
        for (int i = 0; i < fingers.Length; i++) {
            // Jumpscare za kirov.
            string jumpscare = GetRandomCorruptedChars();
            Debug.LogError("Could not read serial info: " + jumpscare  + "\nSerial device might be corrupted!");


            if (i >= fingersAccData.Length) {
                return;
            }


            fingers[i].transform.position = fingersAccData[i] - wristAccData;
        }
    }

    private string GetRandomCorruptedChars()
    {
        string str = "";
        string corrupted = "1̷̡̧̛̼͉͖̐́͌2̵̦̪̥̫̏3̸̨̳̥̤͂4̴͖̭̝̪̜̓̈́͋̌͠5̶̯͈̀͐̑̉͘6̸̨͓̫͕̌̐7̸̛̫͊8̷̬͉͔͓̀9̸͍͛́̂1̶̨̀̕2̴͍̯͒3̶̼͛̾͜4̸͕̔5̵̡̦̋6̶̩͈͌7̷̛̱͚8̴̘̙̅͝9̶̠͝1̴2̵3̷4̶5̷6̵7̵8̵9̸1̷͚̤̀2̵͙̣̐̃3̷͖͛͋4̵͙͋̋ͅ5̵͚͑̿6̷̒ͅ7̸͇̼̂̾8̸̬̀ͅ9̵̤̏̍";
        for (int i = 0; i < 3; i++)
        {
            int length = Random.Range(2, 3);
            for (int j = 0; j < length; j++)
            {
                str += corrupted[Random.Range(0, corrupted.Length)];
            }
            str += " ";
        }
        return str;
    }
}
