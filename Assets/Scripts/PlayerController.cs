﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;

	private Rigidbody2D body;

	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate () {
		float horizontal = Input.GetAxis ("Horizontal");	
		float vertical = Input.GetAxis ("Vertical");

		body.AddForce (new Vector2 (horizontal, vertical) * speed);
	}
}