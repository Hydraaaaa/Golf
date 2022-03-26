using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;

    public Image PowerBar => m_PowerBar;
    public GameObject Scoreboard => m_Scoreboard;

    [SerializeField] Image m_PowerBar;
    [SerializeField] GameObject m_Scoreboard;

    void Awake()
    {
        Instance = this;
    }
}
