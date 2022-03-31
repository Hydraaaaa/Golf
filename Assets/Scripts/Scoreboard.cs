using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameState m_GameState;

    [SerializeField] GameObject m_ScoreboardRoot;

    [SerializeField] Text m_PlayerNamePrefab;
    [SerializeField] RectTransform m_PlayerNameParent;

    [SerializeField] Text m_ScorePrefab;
    [SerializeField] RectTransform m_ScoreParent;

    [SerializeField] int m_ScoreSpacing = 32;

    Dictionary<Player, int> m_ScoreboardIndices = new Dictionary<Player, int>();

    List<Text> m_PlayerNames = new List<Text>();

    List<Text[]> m_Scores = new List<Text[]>();

    void Awake()
    {
        m_GameState.OnPlayerAdded += OnPlayerAdded;
        m_GameState.OnPlayerRemoved += OnPlayerRemoved;
        m_GameState.OnStroke += OnStroke;
        m_GameState.OnPlayerNameSet += OnPlayerNameSet;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_ScoreboardRoot.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            m_ScoreboardRoot.SetActive(false);
        }
    }

    void OnPlayerAdded(Player a_Player)
    {
        Text[] _NewScores = new Text[m_GameState.StageCount];

        for (int i = 0; i < m_GameState.StageCount; i++)
        {
            _NewScores[i] = Instantiate(m_ScorePrefab, m_ScoreParent);

            _NewScores[i].rectTransform.anchoredPosition = new Vector2(i, -m_Scores.Count) * m_ScoreSpacing;
        }

        m_ScoreboardIndices[a_Player] = m_Scores.Count;

        m_Scores.Add(_NewScores);

        Text _PlayerName = Instantiate(m_PlayerNamePrefab, m_PlayerNameParent);

        _PlayerName.rectTransform.anchoredPosition = new Vector2(0, -m_PlayerNames.Count) * m_ScoreSpacing;

        m_PlayerNames.Add(_PlayerName);
    }

    void OnPlayerRemoved(Player a_Player)
    {
        int _PlayerIndex = m_ScoreboardIndices[a_Player];

        Destroy(m_PlayerNames[_PlayerIndex].gameObject);

        m_PlayerNames.RemoveAt(_PlayerIndex);

        for (int i = 0; i < m_GameState.StageCount; i++)
        {
            Destroy(m_Scores[_PlayerIndex][i].gameObject);
        }

        m_Scores.RemoveAt(_PlayerIndex);

        for (int y = _PlayerIndex; y < m_Scores.Count; y++)
        {
            m_PlayerNames[y].rectTransform.anchoredPosition = new Vector2(0, -y) * m_ScoreSpacing;

            for (int x = 0; x < m_GameState.StageCount; x++)
            {
                m_Scores[y][x].rectTransform.anchoredPosition = new Vector2(x, -y) * m_ScoreSpacing;
            }
        }

        m_ScoreboardIndices.Remove(a_Player);
    }

    void OnStroke(Player a_Player, int a_Stroke)
    {
        m_Scores[m_ScoreboardIndices[a_Player]][GameState.Instance.CurrentStage].text = a_Stroke.ToString();
    }

    void OnPlayerNameSet(Player a_Player, string a_Name)
    {
        Debug.Log($"OnPlayerNameSet: {a_Name}");
        m_PlayerNames[m_ScoreboardIndices[a_Player]].text = a_Name;
    }
}
