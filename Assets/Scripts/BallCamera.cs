using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    public Ball Ball { get; set; }

    void Update()
    {
        if (Ball != null)
        {
            transform.position = Ball.transform.position - transform.forward * 5;
        }
    }
}
