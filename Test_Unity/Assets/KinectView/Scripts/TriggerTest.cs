using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
    }
}
