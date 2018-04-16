using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRight : CollisionManager {
    private void OnTriggerEnter(Collider other)
    {
        string hand = other.ToString();
        string[] split = hand.Split(' ');

        if (split[0].Equals("HandRight"))
        {
            SetRightTrue();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        string hand = other.ToString();
        string[] split = hand.Split(' ');

        if (split[0].Equals("HandRight"))
        {
            SetRightFalse();
            Debug.Log("RIGHT - OUT");
        }
    }
}
