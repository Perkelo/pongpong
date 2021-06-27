using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Goal {
    Left,
    Right
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float initialBallSpeedMultiplier = 2f;

    [SerializeField] private Text player1ScoreLabel;
    [SerializeField] private Text player2ScoreLabel;

    private int player1Score = 0;
    private int player2Score = 0;

    private int ballCounter;
    
    void Start()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        player1ScoreLabel.text = "0";
        player2ScoreLabel.text = "0";

        player1Score = 0;
        player2Score = 0;

        ballCounter = 1;
        Rigidbody2D newBall = Instantiate(ball).GetComponent<Rigidbody2D>();
        newBall.position = Vector2.zero;
        newBall.velocity = Vector2.zero;
        newBall.AddForce(new Vector2(Random.Range(0,2) == 0 ? -1 : 1, Random.Range(-1.0f, 1.0f)).normalized * initialBallSpeedMultiplier, ForceMode2D.Impulse);
	}

    public void OnGoalHit(Goal goal, GameObject ball) {
        //Debug.Log(goal);
        ballCounter--;

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

        Destroy(ball);
        if(ballCounter == 0) {
            Invoke(nameof(RestartGame), 2f);
        }
	}

    public void OnBallPlayerCollision(GameObject ball) {
        ballCounter++;
        GameObject newBall = Instantiate(ball);
        newBall.GetComponent<Rigidbody2D>().velocity = (ball.GetComponent<Rigidbody2D>().velocity * 1.08f).Rotate(Random.Range(-10f, 10f));
	}
}
