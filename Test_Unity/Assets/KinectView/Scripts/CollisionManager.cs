using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    private int leftCheck = 0;
    private int rightCheck = 0;

    void Update()
    {

        if (leftCheck + rightCheck == 2)
        {
            Debug.Log("BOTH+++++++++++++++++++++++++++++++++++++++++++++");
        }
    }

    protected void SetLeftTrue() {
        leftCheck = 1;
        Debug.Log(leftCheck);
    }
    protected void SetLeftFalse()
    {
        leftCheck = 0;
    }

    protected void SetRightTrue()
    {
        rightCheck = 1;
        Debug.Log(rightCheck);
    }
    protected void SetRightFalse()
    {
        rightCheck = 0;
    }
}
