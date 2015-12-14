using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GMScript : MonoBehaviour {

	public Text countdownText;
	public float countdown;
	public float ringDelay;
	public GameObject ring;
	public GameObject conv_1;
	public GameObject conv_2;
	GameObject player_1;
	GameObject player_2;

	void Start () {
		player_1 = GameObject.FindGameObjectWithTag("Player 1");
		player_2 = GameObject.FindGameObjectWithTag("Player 2");
	}

	void Update () {
		if (countdownText != null) {
			if (countdown > 0) {
				countdown -= Time.deltaTime;
				countdownText.text = "" + Mathf.Round(countdown);

			} else {
				Transition();
				countdownText.text = "";
			}
		}
	}

	public void StartGame () {
		SceneManager.LoadScene("Tuukka");
	}
	public void Exit () {
		Application.Quit();
	}
	void Transition () {
		if (player_1 != null && player_2 != null) {
			conv_1.GetComponent<ConveyorScript>().convOn = false;
			conv_2.GetComponent<ConveyorScript>().convOn = false;

			ringDelay -= Time.deltaTime;
			if (ringDelay < 0f) {
				player_1.GetComponent<PlayerScript>().conveyorMode = false;
				player_2.GetComponent<PlayerScript>().conveyorMode = false;
				ring.SetActive(true);
			}
		}
	}
}
