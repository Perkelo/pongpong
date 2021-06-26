using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private GameManager manager;

	private void Start() {
		manager = GameObject.FindObjectOfType<GameManager>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if(collision.name.Contains("Goal")) {
			manager.OnGoalHit(collision.name.Contains("Right") ? Goal.Right : Goal.Left, this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.name.Contains("Player")) {
			manager.OnBallPlayerCollision(this.gameObject);
		}
	}
}
