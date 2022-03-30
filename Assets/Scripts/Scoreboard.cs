using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameState m_GameState;

    [SerializeField] RectTransform m_ScoreParent;
    [SerializeField] Text m_ScorePrefab;
    [SerializeField] int m_ScoreSpacing = 32;

    Dictionary<Player, int> m_ScoreboardIndices = new Dictionary<Player, int>();
    List<Text[]> m_Scores = new List<Text[]>();

    void Awake()
    {
        m_GameState.OnPlayerAdded += AddPlayer;
        m_GameState.OnPlayerRemoved += RemovePlayer;
        m_GameState.OnStroke += Stroke;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_ScoreParent.gameObject.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            m_ScoreParent.gameObject.SetActive(false);
        }
    }

    void AddPlayer(Player a_Player)
    {
        Text[] _NewScores = new Text[m_GameState.StageCount];

        for (int i = 0; i < m_GameState.StageCount; i++)
        {
            _NewScores[i] = Instantiate(m_ScorePrefab, m_ScoreParent);

            _NewScores[i].rectTransform.anchoredPosition = new Vector2(i, -m_Scores.Count) * m_ScoreSpacing;
        }

        m_ScoreboardIndices[a_Player] = m_Scores.Count;

        m_Scores.Add(_NewScores);
    }

    void RemovePlayer(Player a_Player)
    {
        int _PlayerIndex = m_ScoreboardIndices[a_Player];

        for (int i = 0; i < m_GameState.StageCount; i++)
        {
            Destroy(m_Scores[_PlayerIndex][i].gameObject);
        }

        m_Scores.RemoveAt(_PlayerIndex);

        for (int y = _PlayerIndex; y < m_Scores.Count; y++)
        {
            for (int x = 0; x < m_GameState.StageCount; x++)
            {
                m_Scores[y][x].rectTransform.anchoredPosition = new Vector2(x, -y) * m_ScoreSpacing;
            }
        }

        m_ScoreboardIndices.Remove(a_Player);
    }

    void Stroke(Player a_Player, int a_Stroke)
    {
        m_Scores[m_ScoreboardIndices[a_Player]][GameState.Instance.CurrentStage].text = a_Stroke.ToString();
    }
}
