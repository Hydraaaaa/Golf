using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class Ball : NetworkBehaviour
{
    public Rigidbody Rigidbody => m_Rigidbody;

    [SerializeField] Rigidbody m_Rigidbody;
    public Player Player { get; set; }

    public override void OnStartAuthority()
    {
        Debug.Log("Ball.OnStartAuthority");
    }
}
