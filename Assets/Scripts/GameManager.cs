using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public BoardManager boardManager;

	private int level = -1;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (this != instance){
			Destroy (gameObject);    
		}

		DontDestroyOnLoad (gameObject);

		boardManager = GetComponent<BoardManager> ();
	}

	void Start() {
		InvokeRepeating ("UpdateObjects", 0.2f, 0.2f);
	}

	public void CheckLevelComplete() {
		if (GameObject.FindWithTag ("Diamond") == null) {
			SceneManager.LoadScene (0);
		}
	}

	void LoadNextLevel() {
		level += 1;
		boardManager.LoadLevel (level);
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
}
