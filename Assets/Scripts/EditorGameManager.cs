using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorGameManager : GameManager {

	private GameObject selected;

	new void Update() {
		base.Update ();

		if (Input.GetKeyDown (KeyCode.C)) {
			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			int x = (int)(pos.x + 0.5f);
			int y = (int)(pos.y + 0.5f);

			selected = boardManager.GetTile (x, y);

			return;
		}

		if (Input.GetKeyDown (KeyCode.V)) {
			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			int x = (int)(pos.x + 0.5f);
			int y = (int)(pos.y + 0.5f);

			boardManager.SetTile (x, y, Instantiate(selected));

			return;
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			int x = (int)(pos.x + 0.5f);
			int y = (int)(pos.y + 0.5f);

			boardManager.DestroyTile (x, y);

			return;
		}

		if (Input.GetKeyDown (KeyCode.S)) {
			boardManager.SaveState ();

			return;
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			boardManager.LoadState ();

			return;
		}
	}
}
