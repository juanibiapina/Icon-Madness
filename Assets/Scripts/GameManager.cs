using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public BoardManager boardManager;

	private int level = 0;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (this != instance){
			Destroy (gameObject);    
		}

		DontDestroyOnLoad (gameObject);

		boardManager = GetComponent<BoardManager> ();
	}

	protected void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene (0);
		}
	}

	public void CheckLevelComplete() {
		if (GameObject.FindWithTag ("Diamond") == null) {
			LoadNextLevel ();
		}
	}

	void LoadNextLevel() {
		level++;
		SceneManager.LoadScene (0);
	}
		
	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
		boardManager.LoadLevel (level);
	}

	void OnEnable(){
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	void OnDisable(){
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}
}
