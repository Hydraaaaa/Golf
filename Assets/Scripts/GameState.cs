using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class GameState : NetworkBehaviour
{
    public static GameState Instance;

    void Awake()
    {
        Instance = this;
    }

    public int StageCount { get { return m_Stages.Length; } }
    public int CurrentStage => m_CurrentStage;
    public Transform CameraStartPos => m_CameraStartPos;

    [SerializeField] Stage[] m_Stages;
    [SerializeField] Ball m_BallPrefab;
    [SerializeField] Transform m_CameraStartPos;

    [SyncVar] int m_CurrentStage = -1;

    readonly SyncDictionary<Player, PlayerInfo> m_Players = new SyncDictionary<Player, PlayerInfo>();

    readonly SyncList<Player> m_PlayersInRound = new SyncList<Player>();

    public event Action<Player> OnPlayerAdded;
    public event Action<Player> OnPlayerRemoved;
    public event Action<Player, int> OnStroke;

    public void AddPlayer(Player a_Player)
    {
        m_Players[a_Player] = new PlayerInfo();
        m_Players[a_Player].Strokes = new int[m_Stages.Length];

        OnPlayerAdded?.Invoke(a_Player);
    }

    public void RemovePlayer(Player a_Player)
    {
        OnPlayerRemoved?.Invoke(a_Player);

        m_Players.Remove(a_Player);
        m_PlayersInRound.Remove(a_Player);
    }

    public void NextStage()
    {
        if (m_CurrentStage == m_Stages.Length - 1)
        {
            // TODO: End Game
        }
        else
        {
            m_CurrentStage++;

            m_PlayersInRound.Clear();

            foreach (var _Player in m_Players.Keys)
            {
                m_PlayersInRound.Add(_Player);

                Ball _Ball = Instantiate(m_BallPrefab, m_Stages[m_CurrentStage].StartPos.position, Quaternion.identity);
                _Ball.Player = _Player;
                NetworkServer.Spawn(_Ball.gameObject, _Player.connectionToClient);

                _Player.OnStartStage(_Ball);
            }
        }
    }

    public void Stroke(Player a_Player)
    {
        m_Players[a_Player].Strokes[m_CurrentStage]++;

        OnStroke?.Invoke(a_Player, m_Players[a_Player].Strokes[m_CurrentStage]);
    }

    public bool IsHoleActive(Hole a_Hole)
    {
        for (int i = 0; i < m_Stages[m_CurrentStage].Holes.Length; i++)
        {
            if (a_Hole == m_Stages[m_CurrentStage].Holes[i])
            {
                return true;
            }
        }

        return false;
    }

    public void HoleTriggered(Player a_Player)
    {
        m_Players[a_Player].HasFinishedCurrentStage = true;

        bool _Finished = true;

        foreach (Player _Player in m_PlayersInRound)
        {
            if (!m_Players[_Player].HasFinishedCurrentStage)
            {
                _Finished = false;
                break;
            }
        }

        if (_Finished)
        {
            NextStage();
        }
    }

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() { }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() {}

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}
