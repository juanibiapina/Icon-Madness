using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
	
	public List<TextAsset> rawLevels;

	public GameObject WallTile;
	public GameObject PlayerTile;
	public GameObject DiamondTile;
	public GameObject BoxTile;
	public GameObject LeftArrowTile;

	private char[] currentLevelRaw;
	private int currentLevelRows;
	private int currentLevelColumns;
	private GameObject[] currentLevel;

	private GameObject[] savedState;

	public void LoadLevel(int levelNumber) {
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
			
		// position camera
		GameObject camera = GameObject.FindWithTag("MainCamera");
		camera.transform.position = new Vector3 (this.currentLevelColumns / 2, this.currentLevelRows / 2, -10);
	}

	public GameObject GetTile(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		return this.currentLevel [pos];
	}

	public void SetTile(int x, int y, GameObject tile) {
		int pos = y * this.currentLevelColumns + x;
		if (this.currentLevel[pos] != null) {
			Destroy (this.currentLevel [pos]);
		}
		tile.transform.position = new Vector3 (x, y, 0);
		this.currentLevel [pos] = tile;
	}

	public void MoveTile(int ox, int oy, int tx, int ty) {
		int origPos = oy * this.currentLevelColumns + ox;
		int targetPos = ty * this.currentLevelColumns + tx;

		GameObject origTile = this.currentLevel [origPos];

		origTile.transform.position = new Vector3 (tx, ty, 0);
		this.currentLevel [targetPos] = origTile;
		this.currentLevel [origPos] = null;
	}

	public void RemoveTile(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		if (this.currentLevel [pos] != null) {
			this.currentLevel [pos].SetActive (false);
		}
		this.currentLevel [pos] = null;
	}

	public void DestroyTile(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		if (this.currentLevel [pos] != null) {
			Destroy (this.currentLevel [pos]);
		}
		this.currentLevel [pos] = null;
	}

	public bool IsOut(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		return pos < 0 || pos >= this.currentLevel.Length;
	}

	public void SaveState() {
		if (savedState != null) {
			foreach (var tile in savedState) {
				if (tile != null) {
					Destroy (tile);
				}
			}
		}

		savedState = new GameObject[this.currentLevelRows * this.currentLevelColumns];

		for (int y = 0; y < this.currentLevelRows; y++) {
			for (int x = 0; x < this.currentLevelColumns; x++) {
				GameObject toCopy = this.currentLevel[y * this.currentLevelColumns + x];

				if (toCopy != null) {
					GameObject instance = Instantiate (toCopy);
					instance.SetActive (false);
					savedState[y * this.currentLevelColumns + x] = instance;
				}
			}
		}
	}

	public void LoadState() {
		foreach (var tile in currentLevel) {
			if (tile != null) {
				Destroy (tile);
			}
		}


		currentLevel = new GameObject[this.currentLevelRows * this.currentLevelColumns];

		for (int y = 0; y < this.currentLevelRows; y++) {
			for (int x = 0; x < this.currentLevelColumns; x++) {
				GameObject toCopy = this.savedState[y * this.currentLevelColumns + x];

				if (toCopy != null) {
					GameObject instance = Instantiate (toCopy);
					instance.SetActive (true);
					currentLevel[y * this.currentLevelColumns + x] = instance;
				}
			}
		}
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
		} else if (type == 'b') {
			toInstantiate = BoxTile;
		} else if (type == 'l') {
			toInstantiate = LeftArrowTile;
		} else if (type == ' ') {
			toInstantiate = null;
		}
		return toInstantiate;
	}
}
