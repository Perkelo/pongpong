using System.Collections.Generic;
using System.Threading;

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

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    private List<GameObject> balls;
    private List<GameObject> pooledBalls;
    private int maxPooledBalls = 10;
    [SerializeField] private float initialBallSpeedMultiplier = 2f;

    [SerializeField] private Text player1ScoreLabel;
    [SerializeField] private Text player2ScoreLabel;

    private int player1Score = 0;
    private int player2Score = 0;

    [SerializeField] public NetMode player1Local;
    [SerializeField] public NetMode player2Local;

    private Thread networkThread = new Thread(CreateRoom);

    [Header("VideoFX")]
    [SerializeField] private float chromaTime = 0.2f;
    [SerializeField] private float chromaIntensity = 0.015f;
    [SerializeField] private float chromaZoomMagnitude = 1.1f;
    [SerializeField] private float shakeXRange = 2f;
    [SerializeField] private float shakeYRange = 2f;

    void Start()
    {
        balls = new List<GameObject>();
        pooledBalls = new List<GameObject>();
        AudioManager.instance.PlayMusic(Music.Level);
        GameObject matchSettingsGO = GameObject.Find("MatchSettings");
        Mode mode;
        if (matchSettingsGO != null)
        {
            MatchSettings matchSettings = matchSettingsGO.GetComponent<MatchSettings>();
            mode = matchSettings.mode;
        }
        else
        {
            mode = Mode.Local;
        }

        switch (mode)
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
                break;
        }

        if (player1Local == NetMode.Local && player2Local == NetMode.Local)
        {
            Invoke(nameof(RestartGame), 2f);
        }
        else
        {
            Invoke(nameof(RestartGame), 2f);
            //networkThread.Start();
            if (player1Local == NetMode.Local)
            {
                //Hosting
            }
            if (player2Local == NetMode.Local)
            {
                //Joining
            }
        }
    }

    private static void CreateRoom()
    {
        MatchmakingConnector.UDPSendMessage("Create");
        MatchmakingConnector.UDPSendMessage("List");
    }

    public void RestartGame()
    {
        AudioManager.instance.PlaySFX(SFX.EndMatch);
        player1ScoreLabel.text = "0";
        player2ScoreLabel.text = "0";

        if (player1Local == NetMode.Local)
        {
            player1Score = 0;
            player2Score = 0;

            GameObject newBall = getBall();
            Rigidbody2D newBallRB2D = newBall.GetComponent<Rigidbody2D>();
            newBallRB2D.position = Vector2.zero;
            newBallRB2D.velocity = Vector2.zero;
            newBallRB2D.AddForce(new Vector2(Random.Range(0, 2) == 0 ? -1 : 1, Random.Range(-1.0f, 1.0f)).normalized * initialBallSpeedMultiplier, ForceMode2D.Impulse);
        }
    }

    public void OnGoalHit(Goal goal, GameObject ball)
    {
        switch (goal)
        {
            case Goal.Right:
                player1Score++;
                player1ScoreLabel.text = $"{player1Score}";
                break;
            case Goal.Left:
                player2Score++;
                player2ScoreLabel.text = $"{player2Score}";
                break;
        }
        removeBall(ball);

        if (balls.Count == 0)
        {
            Invoke(nameof(RestartGame), 2f);
        }
    }

    public void OnBallPlayerCollision(GameObject ball)
    {
        GameObject newBall = getBall();
        newBall.GetComponent<Rigidbody2D>().velocity = (ball.GetComponent<Rigidbody2D>().velocity * 1.08f).Rotate(Random.Range(-10f, 10f));
        newBall.transform.position = ball.transform.position;
        CustomFX.instance.ChromaJump(chromaTime, chromaIntensity, chromaZoomMagnitude);
        CustomFX.instance.Shake(chromaTime, shakeXRange, shakeYRange);
        AudioManager.instance.PlaySFX(SFX.Hit);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    private GameObject getBall()
    {
        GameObject outBall;
        if (pooledBalls.Count == 0)
        {
            outBall = Instantiate(ball);
        }
        else
        {
            outBall = pooledBalls[0];
            pooledBalls.RemoveAt(0);
            outBall.SetActive(true);
            outBall.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        balls.Add(outBall);
        return outBall;
    }

    private void removeBall(GameObject ball)
    {
        ball.SetActive(false);
        if (pooledBalls.Count < maxPooledBalls)
        {
            pooledBalls.Add(ball);
        }
        else
        {
            Destroy(ball);
        }
        balls.Remove(ball);
    }

    public Vector2 GetClosestBallCoordinates(Vector2 position)
    {
        Vector2 result = position;
        float minDistance = Mathf.Infinity;
        foreach(GameObject ball in balls)
        {
            float distance = Vector2.Distance(position, ball.transform.position);
            if (distance < minDistance)
            {
                result = ball.transform.position;
                minDistance = distance;
            }
        }
        return result;
    }
}