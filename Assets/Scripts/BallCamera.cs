using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    public Ball Ball { get; set; }

    void Update()
    {
        transform.eulerAngles += new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);

        if (Ball != null)
        {
            transform.position = Ball.transform.position - transform.forward * 5;
        }
    }
}
