using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomConnectionManager : MonoBehaviour
{
    [SerializeField] private GameObject listItem;
    [SerializeField] private GameObject listContainer;

    void Start()
    {
        listItem.SetActive(false);
        StartCoroutine(UpdateServers());
    }

    private IEnumerator UpdateServers()
    {
        List<ServerEndpoint> servers = MatchmakingConnector.Singleton.GetServers();

        foreach (ServerEndpoint server in servers)
        {
            CreateNewListItem(server);
        }
        yield return new WaitForEndOfFrame();
    }

    private void CreateNewListItem(ServerEndpoint server) {
        GameObject newListItem = Instantiate(listItem);
        newListItem.transform.SetParent(listContainer.transform);
        newListItem.SetActive(true);
        newListItem.transform.localScale = Vector3.one;
        newListItem.GetComponentInChildren<Text>().text = $"{server.IP} : {server.port}";
        newListItem.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            Debug.Log($"{server.IP} : {server.port}");
            GameObject.Find("MatchSettings").GetComponent<MatchSettings>().selectedServer = server;

            SceneManager.LoadSceneAsync("Multiplayer");
        });
    }
}
