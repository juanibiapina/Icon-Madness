using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPushableController : MonoBehaviour {

	public float speed;
		
	private Rigidbody2D body;
	private Dictionary<GameObject, Vector2> collisions;

	void Start () {
		collisions = new Dictionary<GameObject, Vector2> ();
		body = GetComponent<Rigidbody2D> ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Vector2 normal = collision.contacts [0].normal.normalized;

		if (collision.gameObject.CompareTag("Box Pushable")) {
			collisions [collision.gameObject] = normal;

			if (body.velocity.sqrMagnitude > 0) {
				TryMove (body.velocity);
				return;
			}
		}

		if (collision.gameObject.CompareTag("Player")) {
			TryMove (normal);
			return;
		}
	}

	void TryMove(Vector2 direction) {
		foreach(KeyValuePair<GameObject, Vector2> entry in collisions) {
			if (direction.x > 0 && entry.Value.x < 0) {
				direction.x = 0;
				continue;
			}

			if (direction.x < 0 && entry.Value.x > 0) {
				direction.x = 0;
				continue;
			}

			if (direction.y < 0 && entry.Value.y > 0) {
				direction.y = 0;
				continue;
			}

			if (direction.y > 0 && entry.Value.y < 0) {
				direction.y = 0;
				continue;
			}
		}

		direction.Normalize ();

		body.velocity = direction * speed;	
	}

	void OnCollisionStay2D(Collision2D collision) {
		Vector2 normal = collision.contacts [0].normal.normalized;

		if (collision.gameObject.CompareTag("Box Pushable")) {
			collisions [collision.gameObject] = normal;
			return;
		}

		if (collision.gameObject.CompareTag("Player")) {
			TryMove (normal);
			return;
		}
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.CompareTag ("Box Pushable")) {
			collisions.Remove (collision.gameObject);

			return;
		}

		if (collision.gameObject.CompareTag("Player")) {
			body.velocity = Vector2.zero;
			return;
		}
	}
}
