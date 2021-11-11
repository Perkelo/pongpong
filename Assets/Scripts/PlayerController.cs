using System.Collections;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	enum Player
	{
		Player1,
		Player2
	}

	private Rigidbody2D rb2d;
	private GameManager gameManager;

	[SerializeField] private Player playerNum;
	private float m;
	[SerializeField] private float cpuEpsilon = 0.1f;
	[SerializeField] private float cpuDelay = 0.2f;


	[SerializeField] private bool isLocal = true;

	[SerializeField] float speed = 1.0f;

	private void Start()
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();
		switch (playerNum)
		{
			case Player.Player1:
				isLocal = gameManager.player1Local == NetMode.Local;
				break;
			case Player.Player2:
				isLocal = gameManager.player2Local == NetMode.Local;
				break;
		}
		rb2d = GetComponent<Rigidbody2D>();

        if (!isLocal)
        {
			StartCoroutine(AIMovement());
        }
	}

	void FixedUpdate()
	{
		//transform.Translate(0, movement.Value * speed, 0);
		transform.Translate(0, m * speed, 0);
		rb2d.velocity = Vector2.zero;
	}

	public void OnMovePlayer(InputAction.CallbackContext context)
	{
        if (isLocal)
		{
			m = context.ReadValue<float>();
		}
	}

	private IEnumerator AIMovement()
    {
        while (true)
        {
            float y = gameManager.GetClosestBallCoordinates(this.transform.position).y;
			float diff = y - this.transform.position.y;

			m = Mathf.Abs(m - diff) < cpuEpsilon ? 0 : Mathf.Sign(diff);

			yield return new WaitForSeconds(cpuDelay);
        }
    }
}
