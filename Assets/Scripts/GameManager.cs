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

	private int level = -1;

	private char[] currentLevelRaw;
	private int currentLevelRows;
	private int currentLevelColumns;
	private GameObject[] currentLevel;

	private GameObject player;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this){
			Destroy (gameObject);    
		}

		DontDestroyOnLoad (gameObject);
	}

	void LoadNextLevel() {
		level += 1;
		LoadLevel (level);
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		LoadNextLevel ();
	}

	void OnEnable(){
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
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

		int fx = nx + xDir;
		int fy = ny + yDir;

		int playerTile = py * this.currentLevelColumns + px;
		int newTile = ny * this.currentLevelColumns + nx;

		if (newTile < 0 || newTile >= this.currentLevel.Length) {
			return;
		}

		GameObject target = this.currentLevel[newTile];

		if (target == null) {
			player.transform.position = new Vector3(nx, ny, 0);
			this.currentLevel [newTile] = this.currentLevel [playerTile];
			this.currentLevel [playerTile] = null;
			return;
		}

		if (target.CompareTag ("Diamond")) {
			player.transform.position = new Vector3(nx, ny, 0);
			this.currentLevel [newTile].SetActive (false);
			this.currentLevel [newTile] = this.currentLevel [playerTile];
			this.currentLevel [playerTile] = null;

			if (GameObject.FindWithTag ("Diamond") == null) {
				SceneManager.LoadScene (0);
			}
			return;
		}

		if (target.CompareTag ("Box")) {
			int farTile = fy * this.currentLevelColumns + fx;

			if (farTile < 0 || farTile >= this.currentLevel.Length) {
				return;
			}

			if (this.currentLevel [farTile] == null) {
				this.currentLevel [newTile].transform.position = new Vector3 (fx, fy, 0);
				this.currentLevel [farTile] = this.currentLevel [newTile];
				this.currentLevel [newTile] = null;

				player.transform.position = new Vector3(nx, ny, 0);
				this.currentLevel [newTile] = this.currentLevel [playerTile];
				this.currentLevel [playerTile] = null;
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
		} else if (type == ' ') {
			toInstantiate = null;
		}
		return toInstantiate;
	}
}
