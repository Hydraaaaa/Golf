using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class Player : NetworkBehaviour
{
    [SerializeField] Ball m_BallPrefab;
    [SerializeField] Camera m_BallCamera;

    [SyncVar] int m_CurrentStroke;

    Ball m_Ball;

    bool m_IsPlaying;

    bool m_IsAiming;

    float m_Power;

    void Update()
    {
        if (hasAuthority)
        {
            if (m_IsPlaying)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_IsAiming = true;
                    m_Power = 0;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    m_IsAiming = false;

                    if (m_Power > 0)
                    {
                        Vector3 _Forward = m_BallCamera.transform.forward;
                        _Forward.y = 0;
                        _Forward.Normalize();

                        m_Ball.Rigidbody.AddForce(_Forward * m_Power * 3, ForceMode.Impulse);

                        Debug.Log($"Fired at power {m_Power}");
                    }
                }

                if (m_IsAiming)
                {
                    m_Power = Mathf.Clamp(m_Power + Input.GetAxisRaw("Mouse Y"), 0, 10);
                }
                else
                {
                    m_BallCamera.transform.eulerAngles += new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);

                    if (m_Ball != null)
                    {
                        m_BallCamera.transform.position = m_Ball.transform.position - m_BallCamera.transform.forward * 5;
                    }
                }
            }

            if (isServer)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    GameState.Instance.NextStage();
                }
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        // TODO: Wait for next stage to begin, instead of immediately spawning
        CmdSpawnBall();

        m_BallCamera = Instantiate(m_BallCamera, Vector3.one, Quaternion.identity);

        m_IsPlaying = true;
    }

    [Command]
    void CmdSpawnBall()
    {
        m_Ball = Instantiate(m_BallPrefab, new Vector3(4, 1, -20), Quaternion.identity);
        m_Ball.Player = this;
        NetworkServer.Spawn(m_Ball.gameObject, connectionToClient);

        RpcOnBallSpawned(m_Ball);
    }

    [ClientRpc]
    void RpcOnBallSpawned(Ball a_Ball)
    {
        m_Ball = a_Ball;
        a_Ball.Player = this;
    }
}
