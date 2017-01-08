using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public static GameManager instance;

	private int level = 1;

	void Awake() {
		instance = this;
		DontDestroyOnLoad (this);
	}

	public void StarPickedUp() {
		if (GameObject.FindWithTag ("Star") == null) {
			LoadNextLevel ();
		}
	}

	void LoadNextLevel() {
		level += 1;
		SceneManager.LoadSceneAsync ("level" + level);
	}
}
