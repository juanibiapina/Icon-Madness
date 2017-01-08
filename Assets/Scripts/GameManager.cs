using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public static GameManager instance;

	public List<TextAsset> rawLevels;

	public GameObject WallTile;
	public GameObject PlayerTile;
	public GameObject DiamondTile;

	private int level = -1;

	private char[] currentLevelRaw;
	private int currentLevelRows;
	private int currentLevelColumns;
	private GameObject[] currentLevel;

	private GameObject player;

	void Awake() {
		instance = this;
		DontDestroyOnLoad (this);
	}

	void Start() {
		LoadNextLevel ();
	}

	void LoadNextLevel() {
		level += 1;
		LoadLevel (level);
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
		int px = (int)player.transform.position.x;
		int py = (int)player.transform.position.y;

		int nx = px + xDir;
		int ny = py + yDir;

		int new_tile = ny * this.currentLevelColumns + nx;
		int player_tile = py * this.currentLevelColumns + px;

		if (new_tile < 0 || new_tile >= this.currentLevel.Length) {
			return;
		}

		GameObject neighbour = this.currentLevel[new_tile];

		if (neighbour == null) {
			player.transform.position = new Vector3(nx, ny, 0);
			this.currentLevel [new_tile] = this.currentLevel [player_tile];
			this.currentLevel [player_tile] = null;
//			int far_tile = (ny + yDir) * levelManager.cols + (nx + xDir);
//
//			if ((far_tile > -1) && (far_tile < tiles.Length) && (tiles[far_tile].tag != "Wall")) {
//				// we can push!
//
//				// push the wall
//				SwapTiles(new_tile, far_tile);
//				neighbour.transform.position = new Vector3(nx + xDir, ny + yDir, 0);
//
//				// move the player
//				SwapTiles(player_tile, new_tile);
//				player.transform.position = new Vector3(nx, ny, 0);
//
//				// a wall moved so check for victory
//				CheckVictory();
//			}
//
//			return;
		}
	}

	void LoadLevel(int levelNumber) {
		// load raw data into an array of chars
		TextAsset rawLevel = rawLevels [levelNumber];

		string text = rawLevel.text;

		this.currentLevelRows = 0;
		this.currentLevelColumns = 0;

		int index = 0;

		while (index < text.Length) {
			char ch = text[index];

			if (ch == '\n') {
				this.currentLevelRows++;
			}

			index++;
		}

		this.currentLevelRaw = text.Replace ("\n", "").ToCharArray();
		this.currentLevelColumns = this.currentLevelRaw.Length / this.currentLevelRows;

		// instantiate game objects
		currentLevel = new GameObject[this.currentLevelRows * this.currentLevelColumns];

		for (int y = 0; y < this.currentLevelRows; y++) {
			for (int x = 0; x < this.currentLevelColumns; x++) {
				GameObject toInstantiate = TileForPosition (x, y);

				if (toInstantiate != null) {
					GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					currentLevel[y*this.currentLevelColumns + x] = instance;
				}
			}
		}

		// find player
		player = GameObject.FindWithTag("Player");
	}

	GameObject TileForPosition(int col, int row) {
		row = this.currentLevelRows - row - 1;

		char type = this.currentLevelRaw[row*this.currentLevelColumns + col];
		GameObject toInstantiate = null;

		if (type == '#') {
			toInstantiate = WallTile;
		} else if (type == 'P') {
			toInstantiate = PlayerTile;
		} else if (type == 'D') {
			toInstantiate = DiamondTile;
		} else if (type == ' ') {
			toInstantiate = null;
		}
		return toInstantiate;
	}
}
