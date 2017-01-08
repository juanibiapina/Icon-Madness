using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public List<TextAsset> rawLevels;

	public GameObject WallTile;
	public GameObject PlayerTile;
	public GameObject DiamondTile;
	public GameObject BoxTile;
	public GameObject LeftArrowTile;

	private int level = -1;

	private char[] currentLevelRaw;
	private int currentLevelRows;
	private int currentLevelColumns;
	private GameObject[] currentLevel;

	private GameObject player;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (this != instance){
			Destroy (gameObject);    
		}

		DontDestroyOnLoad (gameObject);
	}

	void Start() {
		InvokeRepeating ("UpdateObjects", 0.2f, 0.2f);
	}

	void UpdateObjects() {
		for (int y = 0; y < this.currentLevelRows; y++) {
			for (int x = 0; x < this.currentLevelColumns; x++) {
				GameObject tile = GetTile (x, y);

				if (tile == null) {
					continue;
				}

				if (tile.CompareTag("Left Arrow")) {
					if (IsOut (x - 1, y)) {
						continue;
					}

					if (GetTile (x - 1, y) == null) {
						MoveTile (x, y, x - 1, y);
						continue;
					}

					if (GetTile (x - 1, y).CompareTag ("Left Arrow") || GetTile (x - 1, y).CompareTag ("Player")) {
						if (GetTile (x, y - 1) == null) {
							if (GetTile (x - 1, y - 1) == null) {
								MoveTile (x, y, x - 1, y - 1);
								continue;
							}
						}

						if (GetTile (x, y + 1) == null) {
							if (GetTile (x - 1, y + 1) == null) {
								MoveTile (x, y, x - 1, y + 1);
								continue;
							}
						}
					}

					continue;
				}
			}
		}
	}

	void LoadNextLevel() {
		level += 1;
		LoadLevel (level);
	}

	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
		LoadNextLevel ();
	}

	void OnEnable(){
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
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

	void MoveTile(int ox, int oy, int tx, int ty) {
		int origPos = oy * this.currentLevelColumns + ox;
		int targetPos = ty * this.currentLevelColumns + tx;

		GameObject origTile = this.currentLevel [origPos];

		origTile.transform.position = new Vector3 (tx, ty, 0);
		this.currentLevel [targetPos] = origTile;
		this.currentLevel [origPos] = null;
	}

	void RemoveTile(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		this.currentLevel [pos].SetActive (false);
		this.currentLevel [pos] = null;
	}

	bool IsOut(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		return pos < 0 || pos >= this.currentLevel.Length;
	}

	void CheckLevelComplete() {
		if (GameObject.FindWithTag ("Diamond") == null) {
			SceneManager.LoadScene (0);
		}
	}

	GameObject GetTile(int x, int y) {
		int pos = y * this.currentLevelColumns + x;
		return this.currentLevel [pos];
	}

	void AttemptMove (int xDir, int yDir) {
		int px = (int)player.transform.position.x;
		int py = (int)player.transform.position.y;

		int nx = px + xDir;
		int ny = py + yDir;

		if (IsOut (nx, ny)) {
			return;
		}
			
		GameObject targetTile = GetTile (nx, ny);

		if (targetTile == null) {
			MoveTile (px, py, nx, ny);
			return;
		}

		if (targetTile.CompareTag ("Diamond")) {
			RemoveTile (nx, ny);
			MoveTile (px, py, nx, ny);
			CheckLevelComplete ();

			return;
		}

		if (targetTile.CompareTag ("Box")) {
			int fx = nx + xDir;
			int fy = ny + yDir;

			if (IsOut(fx, fy)) {
				return;
			}

			if (GetTile (fx, fy) == null) {
				MoveTile (nx, ny, fx, fy);
				MoveTile (px, py, nx, ny);
			}

			return;
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

		// position camera
		GameObject camera = GameObject.FindWithTag("MainCamera");
		camera.transform.position = new Vector3 (this.currentLevelColumns / 2, this.currentLevelRows / 2, -10);
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
