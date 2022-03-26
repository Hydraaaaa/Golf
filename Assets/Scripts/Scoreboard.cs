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

    List<Text[]> m_Scores = new List<Text[]>();

    void Awake()
    {
        m_GameState.OnPlayerAdded += AddPlayer;
        m_GameState.OnPlayerRemoved += RemovePlayer;
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

    public void AddPlayer(Player a_Player)
    {
        Text[] _NewScores = new Text[m_GameState.StageCount];

        for (int i = 0; i < m_GameState.StageCount; i++)
        {
            _NewScores[i] = Instantiate(m_ScorePrefab, m_ScoreParent);

            _NewScores[i].rectTransform.anchoredPosition = new Vector2(i, -m_Scores.Count) * m_ScoreSpacing;
        }

        m_Scores.Add(_NewScores);
    }

    public void RemovePlayer(Player a_Player, int a_Index)
    {
        for (int i = 0; i < m_GameState.StageCount; i++)
        {
            Destroy(m_Scores[a_Index][i].gameObject);
        }

        m_Scores.RemoveAt(a_Index);

        for (int y = a_Index; y < m_Scores.Count; y++)
        {
            for (int x = 0; x < m_GameState.StageCount; x++)
            {
                m_Scores[y][x].rectTransform.anchoredPosition = new Vector2(x, -y) * m_ScoreSpacing;
            }
        }
    }
}
