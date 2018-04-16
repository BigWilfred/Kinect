using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionLeft : CollisionManager
{
    private void OnTriggerEnter(Collider other)
    {
        string hand = other.ToString();
        string[] split = hand.Split(' ');

        if (split[0].Equals("HandLeft"))
        {
            SetLeftTrue();
        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string hand = other.ToString();
        string[] split = hand.Split(' ');

        if (split[0].Equals("HandLeft"))
        {
            SetLeftFalse();
            Debug.Log("Left - OUT");
        }
    }
}
