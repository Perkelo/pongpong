using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Goal {
    Left,
    Right
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private float initialBallSpeedMultiplier = 2f;

    private int ballCounter;
    
    void Start()
    {
        RestartGame();
    }

    public void RestartGame() {
        ballCounter = 1;
        Rigidbody2D newBall = Instantiate(ball).GetComponent<Rigidbody2D>();
        newBall.position = Vector2.zero;
        newBall.velocity = Vector2.zero;
        newBall.AddForce(new Vector2(Random.Range(0,2) == 0 ? -1 : 1, Random.Range(-1.0f, 1.0f)).normalized * initialBallSpeedMultiplier, ForceMode2D.Impulse);
	}

    public void OnGoalHit(Goal goal, GameObject ball) {
        //Debug.Log(goal);
        ballCounter--;
        Destroy(ball);
        if(ballCounter == 0) {
            RestartGame();
        }
	}

    public void OnBallPlayerCollision(GameObject ball) {
        ballCounter++;
        GameObject newBall = Instantiate(ball);
        newBall.GetComponent<Rigidbody2D>().velocity = (ball.GetComponent<Rigidbody2D>().velocity * 1.08f).Rotate(Random.Range(-10f, 10f));
	}
}
