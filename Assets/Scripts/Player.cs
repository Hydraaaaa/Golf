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
    [SerializeField] Camera m_BallCamera;
    [SerializeField] float m_MaxPower = 10.0f;

    public int Index = 0;

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
                if (m_Ball.Rigidbody.velocity.magnitude < 0.0001f)
                {
                    if (Input.GetMouseButtonDown(0) &&
                        !UI.Instance.IsEscapeMenuOpen)
                    {
                        m_IsAiming = true;
                        m_Power = 0;

                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }

                    if (Input.GetMouseButtonUp(0) &&
                        m_IsAiming)
                    {
                        m_IsAiming = false;

                        if (m_Power > 0)
                        {
                            Vector3 _Forward = m_BallCamera.transform.forward;
                            _Forward.y = 0;
                            _Forward.Normalize();

                            m_Ball.Rigidbody.AddForce(_Forward * m_Power * 3, ForceMode.Impulse);

                            CmdStroke();

                            Debug.Log($"Fired at power {m_Power}");
                        }

                        UI.Instance.PowerBar.fillAmount = 0;
                    }
                }
                else // Future proofing for cases where the ball can be displaced before a swing
                {
                    m_IsAiming = false;
                    UI.Instance.PowerBar.fillAmount = 0;
                }

                if (m_IsAiming)
                {
                    m_Power = Mathf.Clamp(m_Power + Input.GetAxisRaw("Mouse Y"), 0, m_MaxPower);

                    UI.Instance.PowerBar.fillAmount = m_Power / m_MaxPower;
                }
                else
                {
                    m_BallCamera.transform.eulerAngles += new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);
                }

                if (m_Ball != null)
                {
                    m_BallCamera.transform.position = m_Ball.transform.position - m_BallCamera.transform.forward * 5;
                }
            }

            if (isServer)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (GameState.Instance.CurrentStage == -1)
                    {
                        GameState.Instance.NextStage();
                    }
                }
            }
        }
    }

    [Command]
    void CmdStroke()
    {
        GameState.Instance.Stroke(this);
    }

    public override void OnStartServer()
    {
        GameState.Instance.AddPlayer(this);
    }

    public override void OnStopServer()
    {
        GameState.Instance.RemovePlayer(this);
    }

    public override void OnStartLocalPlayer()
    {
        m_BallCamera = Instantiate(m_BallCamera);

        Transform _Transform = GameState.Instance.CameraStartPos;

        m_BallCamera.transform.position = _Transform.position;
        m_BallCamera.transform.rotation = _Transform.rotation;

        m_IsPlaying = false;

        CmdSetPlayerName(MainMenu.PlayerName);
    }

    [Command]
    void CmdSetPlayerName(string a_Name)
    {
        Debug.Log($"SetPlayerName: {a_Name}");
        GameState.Instance.SetPlayerName(this, a_Name);
    }

    [ClientRpc]
    public void OnStartStage(Ball a_Ball)
    {
        m_Ball = a_Ball;
        a_Ball.Player = this;

        m_IsPlaying = true;

        m_BallCamera.transform.rotation = a_Ball.transform.rotation;
    }

    [Command]
    public void CmdHoleTriggered(Player a_Player)
    {
        GameState.Instance.HoleTriggered(a_Player);
    }
}
