using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag ("Player")) {
			gameObject.SetActive (false);
			GameManager.instance.StarPickedUp ();
		}
	}
}
