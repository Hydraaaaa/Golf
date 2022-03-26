using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    void OnTriggerEnter(Collider a_Other)
    {
        Ball _Ball = a_Other.GetComponent<Ball>();

        if (_Ball != null &&
            _Ball.Player.hasAuthority &&
            GameState.Instance.IsHoleActive(this))
        {
            _Ball.Player.CmdHoleTriggered(_Ball.Player);
        }
    }
}
