using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour {

	private BoardManager boardManager;

	void Start() {
		boardManager = GameManager.instance.boardManager;
		InvokeRepeating ("Updater", 0.2f, 0.2f);
	}

	void Updater() {
		int x = (int)transform.position.x;
		int y = (int)transform.position.y;

		if (boardManager.IsOut (x - 1, y)) {
			return;
		}

		if (boardManager.GetTile (x - 1, y) == null) {
			boardManager.MoveTile (x, y, x - 1, y);
			return;
		}

		if (boardManager.GetTile (x - 1, y).CompareTag ("Left Arrow")) {
			if (boardManager.GetTile (x, y - 1) == null) {
				if (boardManager.GetTile (x - 1, y - 1) == null) {
					boardManager.MoveTile (x, y, x - 1, y - 1);
					return;
				}
			}

			if (boardManager.GetTile (x, y + 1) == null) {
				if (boardManager.GetTile (x - 1, y + 1) == null) {
					boardManager.MoveTile (x, y, x - 1, y + 1);
					return;
				}
			}
		}
	}
}