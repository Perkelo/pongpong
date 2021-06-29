using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Goal {
    Left,
    Right
}

public enum NetMode
{
    Local,
    Online
}

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float initialBallSpeedMultiplier = 2f;

    [SerializeField] private Text player1ScoreLabel;
    [SerializeField] private Text player2ScoreLabel;

    private NetworkVariableInt player1Score = new NetworkVariableInt(
        new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.ServerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        });
    private NetworkVariableInt player2Score = new NetworkVariableInt(
     new NetworkVariableSettings
     {
         WritePermission = NetworkVariablePermission.ServerOnly,
         ReadPermission = NetworkVariablePermission.Everyone
     });

    private int ballCounter;

    [SerializeField] public NetMode player1Local;
    [SerializeField] public NetMode player2Local;
    
    void Start()
    {
        GameObject matchSettingsGO = GameObject.Find("MatchSettings");
        if(matchSettingsGO != null)
        {
            MatchSettings matchSettings = matchSettingsGO.GetComponent<MatchSettings>();

            switch (matchSettings.mode)
            {
                case Mode.Local:
                    player1Local = NetMode.Local;
                    player2Local = NetMode.Local;
                    break;
                case Mode.Host:
                    player1Local = NetMode.Local;
                    player2Local = NetMode.Online;
                    break;
                case Mode.Join:
                    player1Local = NetMode.Online;
                    player2Local = NetMode.Local;
                    NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = matchSettings.selectedServer.IP;
                    NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectPort = matchSettings.selectedServer.port;
                    Debug.Log($"Connecting to {matchSettings.selectedServer}");
                    break;
            }
        }

        if (player1Local == NetMode.Local && player2Local == NetMode.Local)
        {
            RestartGame();
        }
        else
        {
            if(player1Local == NetMode.Local)
            {
                NetworkManager.Singleton.StartServer();
                NetworkManager.Singleton.OnClientConnectedCallback += delegate(ulong clientId)
                {
                    GameObject.Find("Player2").GetComponent<NetworkObject>().ChangeOwnership(clientId);
                    RestartGame();
                };
            }
            if(player2Local == NetMode.Local)
            {
                NetworkManager.Singleton.StartClient();
                //NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress;

                player1Score.OnValueChanged += delegate (int oldValue, int newValue)
                {
                    player1ScoreLabel.text = $"{newValue}";
                };
                player2Score.OnValueChanged += delegate (int oldValue, int newValue)
                {
                    player2ScoreLabel.text = $"{newValue}";
                };
            }
        }
    }

    public void RestartGame()
    {
        player1ScoreLabel.text = "0";
        player2ScoreLabel.text = "0";

        ballCounter = 1;

        if (player1Local == NetMode.Local)
        {
            player1Score.Value = 0;
            player2Score.Value = 0;

            Rigidbody2D newBall = Instantiate(ball).GetComponent<Rigidbody2D>();
            newBall.position = Vector2.zero;
            newBall.velocity = Vector2.zero;
            newBall.AddForce(new Vector2(Random.Range(0, 2) == 0 ? -1 : 1, Random.Range(-1.0f, 1.0f)).normalized * initialBallSpeedMultiplier, ForceMode2D.Impulse);

            newBall.GetComponent<NetworkObject>().Spawn();
        }
	}

    public void OnGoalHit(Goal goal, GameObject ball) {
        //Debug.Log(goal);
        ballCounter--;

        if (player1Local == NetMode.Local)
        {
            switch (goal)
            {
                case Goal.Right:
                    player1Score.Value++;
                    player1ScoreLabel.text = $"{player1Score.Value}";
                    break;
                case Goal.Left:
                    player2Score.Value++;
                    player2ScoreLabel.text = $"{player2Score.Value}";
                    break;
            }
            Destroy(ball);
        }

        if(ballCounter == 0) {
            Invoke(nameof(RestartGame), 2f);
        }
	}

    public void OnBallPlayerCollision(GameObject ball) {
        ballCounter++;
        if (player1Local == NetMode.Local)
        {
            GameObject newBall = Instantiate(ball);
            newBall.GetComponent<Rigidbody2D>().velocity = (ball.GetComponent<Rigidbody2D>().velocity * 1.08f).Rotate(Random.Range(-10f, 10f));
            newBall.GetComponent<NetworkObject>().Spawn();
        }
	}

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
