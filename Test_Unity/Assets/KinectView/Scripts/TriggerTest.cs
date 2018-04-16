using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour {

    private bool leftOn = false;
    private bool rightOn = false;
    private int count = 0;
    private void OnTriggerEnter(Collider other)
    {
        string hand = other.ToString();
        string[] split = hand.Split(' ');

        if (split[0].Equals("HandLeft")) {  
            leftOn = true;
        }
        if (split[0].Equals("HandRight"))
        { 
            rightOn = true;
        }

        if (leftOn && rightOn) {
            Debug.Log("BOTH ON JEFFF **********");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        string hand = other.ToString();
        string[] split = hand.Split(' ');

        if (split[0].Equals("HandLeft"))
        {
            //leftOn = false;
        }
        if (split[0].Equals("HandRight"))
        {
            //rightOn = false;
        }
    }
        void Update()
    {
        //Debug.Log("LEFT " + count + ": " + leftOn);
        //Debug.Log("RIGHT " + count + ": " + rightOn);
        count++;
    }
}
