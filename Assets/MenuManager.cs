using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public string map;

	public void StartGame () {
		SceneManager.LoadScene(map);
	}
	public void Exit () {
		Application.Quit();
	}
}
