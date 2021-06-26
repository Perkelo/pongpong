using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	enum Player {
		Player1,
		Player2
	}

	private Rigidbody2D rb2d;

	[SerializeField] private Player playerNum;
	private float movement;

	[SerializeField] float speed = 1.0f;

	private void Start() {
		rb2d = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate() {
		transform.Translate(0, movement * speed, 0);
		rb2d.velocity = Vector2.zero;
	}

	public void OnMovePlayer1(InputAction.CallbackContext context) {
		movement = context.ReadValue<float>();
	}

	public void OnMovePlayer2(InputAction.CallbackContext context) {
		movement = context.ReadValue<float>();
	}
}
