using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour
{
    public void OnLocalClicked()
    {
        GameObject.Find("MatchSettings").GetComponent<MatchSettings>().mode = Mode.Local;
        SceneManager.LoadSceneAsync("Multiplayer");
    }

    public void OnCreateRoomClicked()
    {
        GameObject.Find("MatchSettings").GetComponent<MatchSettings>().mode = Mode.Host;
        SceneManager.LoadSceneAsync("Multiplayer");
    }

    public void OnJoinRoomClicked()
    {
        GameObject.Find("MatchSettings").GetComponent<MatchSettings>().mode = Mode.Join;
        SceneManager.LoadSceneAsync("Room Choosing");
    }
}
