using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Local,
    Host,
    Join
}

public class MatchSettings : MonoBehaviour
{
    public Mode mode;
    public ServerEndpoint selectedServer;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
