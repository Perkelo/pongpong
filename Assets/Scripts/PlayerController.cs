using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
	enum Player
	{
		Player1,
		Player2
	}

	private Rigidbody2D rb2d;

	[SerializeField] private Player playerNum;
	private NetworkVariableFloat movement = new NetworkVariableFloat(
		new NetworkVariableSettings
		{
			WritePermission = NetworkVariablePermission.Everyone,
			ReadPermission = NetworkVariablePermission.Everyone
		});
	private float m;


	private bool isLocal = true;

	[SerializeField] float speed = 1.0f;

	private void Start()
	{
		switch (playerNum)
		{
			case Player.Player1:
				isLocal = GameObject.FindObjectOfType<GameManager>().player1Local == NetMode.Local;
				break;
			case Player.Player2:
				isLocal = GameObject.FindObjectOfType<GameManager>().player2Local == NetMode.Local;
				break;
		}
		rb2d = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		//transform.Translate(0, movement.Value * speed, 0);
		transform.Translate(0, m * speed, 0);
		rb2d.velocity = Vector2.zero;
	}

	public void OnMovePlayer(InputAction.CallbackContext context)
	{
		//movement.Value = context.ReadValue<float>();
		m = context.ReadValue<float>();
	}
}
