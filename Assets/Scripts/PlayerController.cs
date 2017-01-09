using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private BoardManager boardManager;

	void Start() {
		boardManager = GameManager.instance.boardManager;
	}

	void Update() {
		int horizontal = 0;
		int vertical = 0;

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			horizontal = 1;
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			horizontal = -1;
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			vertical = 1;
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			vertical = -1;
		}

		if (horizontal != 0 || vertical != 0) {
			AttemptMove (horizontal, vertical);
		}
	}

	void AttemptMove (int xDir, int yDir) {
		int px = (int)transform.position.x;
		int py = (int)transform.position.y;

		int nx = px + xDir;
		int ny = py + yDir;

		if (boardManager.IsOut (nx, ny)) {
			return;
		}

		GameObject targetTile = boardManager.GetTile (nx, ny);

		if (targetTile == null) {
			boardManager.MoveTile (px, py, nx, ny);
			return;
		}

		if (targetTile.CompareTag ("Diamond")) {
			boardManager.RemoveTile (nx, ny);
			boardManager.MoveTile (px, py, nx, ny);
			GameManager.instance.CheckLevelComplete ();

			return;
		}

		if (targetTile.CompareTag ("Box")) {
			int fx = nx + xDir;
			int fy = ny + yDir;

			if (boardManager.IsOut(fx, fy)) {
				return;
			}

			if (boardManager.GetTile (fx, fy) == null) {
				boardManager.MoveTile (nx, ny, fx, fy);
				boardManager.MoveTile (px, py, nx, ny);
			}

			return;
		}
	}
}
