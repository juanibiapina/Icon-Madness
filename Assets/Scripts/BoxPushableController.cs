using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPushableController : MonoBehaviour {
		
	private Rigidbody2D body;

	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			body.constraints = RigidbodyConstraints2D.FreezeRotation;
		}
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			body.constraints = RigidbodyConstraints2D.FreezeAll;
			body.velocity = Vector2.zero;
		}
	}
}