using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIScript : MonoBehaviour {

	public void UIStart () {
		SceneManager.LoadScene("Tuukka");
	}

	public void Exit () {
		Application.Quit();
	}
}
