using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MainMenu : MonoBehaviour
{
    public static string PlayerName => s_PlayerName;

    static string s_PlayerName = "";

    [SerializeField] NetworkManager m_NetworkManager;

    [SerializeField] TMP_InputField m_IPInput;
    [SerializeField] TMP_InputField m_NameInput;

    void Awake()
    {
        if (s_PlayerName == "")
        {
            m_NameInput.text = "Player";
        }
        else
        {
            m_NameInput.text = s_PlayerName;
        }
    }

    public void OnHost()
    {
        s_PlayerName = m_NameInput.text;
        m_NetworkManager.StartHost();
    }

    public void OnJoin()
    {
        s_PlayerName = m_NameInput.text;
        m_NetworkManager.networkAddress = m_IPInput.text;
        m_NetworkManager.StartClient();
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
