using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MainMenu : MonoBehaviour
{
    [SerializeField] NetworkManager m_NetworkManager;

    [SerializeField] TMP_InputField m_IPInput;
    [SerializeField] TMP_InputField m_NameInput;

    public void OnHost()
    {
        m_NetworkManager.StartHost();
    }

    public void OnJoin()
    {
        m_NetworkManager.networkAddress = m_IPInput.text;
        m_NetworkManager.StartClient();
    }
}
