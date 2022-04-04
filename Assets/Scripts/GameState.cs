using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class GameState : NetworkBehaviour
{
    public static GameState Instance;

    public int StageCount { get { return m_Stages.Length; } }
    public int CurrentStage => m_CurrentStage;
    public Transform CameraStartPos => m_CameraStartPos;
    public TextMeshProUGUI PregameText => m_PregameText;
    public Stage[] Stages => m_Stages;

    [SerializeField] Stage[] m_Stages;
    [SerializeField] Ball m_BallPrefab;
    [SerializeField] Transform m_CameraStartPos;
    [SerializeField] TextMeshProUGUI m_PregameText;

    [SyncVar] int m_CurrentStage = -1;

    readonly SyncDictionary<Player, PlayerInfo> m_Players = new SyncDictionary<Player, PlayerInfo>();

    readonly SyncList<Player> m_PlayersInRound = new SyncList<Player>();

    public event Action<Player> OnPlayerAdded;
    public event Action<Player> OnPlayerRemoved;
    public event Action<Player, int> OnStroke;
    public event Action<Player, string> OnPlayerNameSet;

    void Awake()
    {
        Instance = this;
    }

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

                Ball _Ball = Instantiate(m_BallPrefab, m_Stages[m_CurrentStage].StartPos.position, m_Stages[m_CurrentStage].StartPos.rotation);
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

    public void SetPlayerName(Player a_Player, string a_Name)
    {
        m_Players[a_Player].Name = a_Name;

        OnPlayerNameSet?.Invoke(a_Player, a_Name);
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            m_PregameText.text = "Press Space to Start";
        }
        else
        {
            m_PregameText.text = "Waiting for Host";
        }
    }
}
