using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;

    public Image PowerBar => m_PowerBar;
    public GameObject Scoreboard => m_Scoreboard;
    public bool IsEscapeMenuOpen { get; private set; }

    [SerializeField] Image m_PowerBar;
    [SerializeField] GameObject m_Scoreboard;
    [SerializeField] GameObject m_EscapeMenu;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_EscapeMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            IsEscapeMenuOpen = true;
        }
    }

    public void OnReturnToGame()
    {
        m_EscapeMenu.SetActive(false);
        IsEscapeMenuOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnQuit()
    {
        if (GameState.Instance.isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
