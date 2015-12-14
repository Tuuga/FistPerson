using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GMScript : MonoBehaviour {

	public string map;
	public Text countdownText;
	public Text p1Text;
	public Text p2Text;
	public GameObject endScreen;
	public float countdown;
	public float ringDelay;
	public GameObject ring;
	public GameObject conv_1;
	public GameObject conv_2;
	GameObject player_1;
	GameObject player_2;
	float p1hp;
	float p2hp;

	void Start () {
		player_1 = GameObject.FindGameObjectWithTag("Player 1");
		player_2 = GameObject.FindGameObjectWithTag("Player 2");
	}

	void Update () {
		p1hp = player_1.GetComponent<PlayerScript>().health;
		p2hp = player_2.GetComponent<PlayerScript>().health;

		if (p1hp <= 0) {
			GameEnd("Player 2");
		} else if (p2hp <= 0) {
			GameEnd("Player 1");
		} else {
			p1Text.text = "";
			p2Text.text = "";
        }

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
		SceneManager.LoadScene(map);
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
	void GameEnd (string player) {
		if (player == "Player 1") {
			p1Text.text = "You won!";
			p2Text.text = "You lost...";
		} else {
			p1Text.text = "You lost...";
			p2Text.text = "You won!";
		}
		Time.timeScale = 0;
		endScreen.SetActive(true);
	}
	public void Restart () {
		Time.timeScale = 1;
		SceneManager.LoadScene(map);
	}
}
