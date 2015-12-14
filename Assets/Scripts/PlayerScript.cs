using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public KeyCode leftInput;
	public KeyCode rightInput;

	public GameObject hitEffect;
	public GameObject rightFist;
	public GameObject leftFist;
	public GameObject pivot;
	public bool conveyorMode;
	public float health;
	public float opponentHealth;
	public float punchDamage;
	public float chargeMaxTime;
	float currentChargeTime;
	public float blockInputTime;
	public float dodgeInputTime;
	public float startupTime;
	public float staggerTime;
	public float recoveryTime;
	public float blockTime;
	public float dodgeTime;
	public bool showStateLogs;

	[HideInInspector]
	public bool leftDodge;
	[HideInInspector]
	public bool rightDodge;
	bool opponentLeftDodge;
	bool opponentRightDodge;

	GameObject opponent;
	enum FistState { Resting, Input, Block, Dodge, Startup, Stagger, Recovery };
	FistState leftFistState;
	FistState rightFistState;
	FistState opponentLeftFistState;
	FistState opponentRightFistState;
	float leftTimer;
	float rightTimer;
	public Transform[] fistStatePos;

	void Start () {
		if (tag == "Player 1") {
			opponent = GameObject.FindGameObjectWithTag("Player 2");
		} else {
			opponent = GameObject.FindGameObjectWithTag("Player 1");
		}
	}

	void Update () {
		opponentLeftFistState = opponent.GetComponent<PlayerScript>().leftFistState;
		opponentRightFistState = opponent.GetComponent<PlayerScript>().rightFistState;
		opponentLeftDodge = opponent.GetComponent<PlayerScript>().leftDodge;
		opponentRightDodge = opponent.GetComponent<PlayerScript>().rightDodge;

		UpdateFist(ref leftFistState, ref leftTimer, rightFistState, ref rightTimer, leftInput, leftFist);      //LeftUpdate
		UpdateFist(ref rightFistState, ref rightTimer, leftFistState, ref leftTimer, rightInput, rightFist);    //RightUpdate
	}

	void UpdateFist (ref FistState currentState, ref float currentTimer, FistState other, ref float otherTimer, KeyCode currentKey, GameObject fistObject) {

		if (currentState == FistState.Resting) {            //Resting
			MoveTo(fistStatePos[0].position, fistStatePos[1].position, fistObject, recoveryTime);
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			if (Input.GetKeyDown(currentKey)) {
				currentState = FistState.Input;
				currentTimer = 0;

			}
		} else if (currentState == FistState.Input) {       //Input
			MoveTo(fistStatePos[6].position, fistStatePos[7].position, fistObject, chargeMaxTime);
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			if (Input.GetKey(currentKey)) {
				currentTimer += Time.deltaTime;
				currentChargeTime = currentTimer;
				if (currentTimer < blockInputTime && other == FistState.Input && otherTimer < blockInputTime && conveyorMode == false || other == FistState.Block) {
					currentState = FistState.Block;
					currentTimer = 0;
				} else if (currentTimer < dodgeInputTime && otherTimer < dodgeInputTime && other == FistState.Input && conveyorMode == false || other == FistState.Dodge) {
					currentState = FistState.Dodge;
				}
			} else if (other != FistState.Input && other != FistState.Startup) {
				currentTimer = 0;
				currentState = FistState.Startup;
			} else {
				currentTimer = 0;
				currentState = FistState.Resting;
			}
		} else if (currentState == FistState.Block) {       //Block
			MoveTo(fistStatePos[4].position, fistStatePos[5].position, fistObject, blockTime / 8);
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;

			if (currentTimer > blockTime) {
				currentTimer = 0;
				MoveTo(fistStatePos[0].position, fistStatePos[1].position, fistObject, blockTime / 8);
				currentState = FistState.Resting;
			}

		} else if (currentState == FistState.Dodge) {       //Dodge
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > dodgeTime) {
				currentTimer = 0;
				currentState = FistState.Resting;
				leftDodge = false;
				rightDodge = false;
			}
			Dodge(ref currentTimer, otherTimer, currentKey, ref currentState);

		} else if (currentState == FistState.Startup) {     //Startup
			MoveTo(fistStatePos[2].position, fistStatePos[3].position, fistObject, startupTime);
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > startupTime) {
				currentTimer = 0;
				if (opponentLeftFistState == FistState.Block) {
					currentState = FistState.Stagger;
					other = FistState.Stagger;
				} else if ((currentKey == leftInput && opponentLeftDodge) || (currentKey == rightInput && opponentRightDodge)) {
					currentState = FistState.Stagger;
					other = FistState.Stagger;
				} else {
					if (conveyorMode == false) {
						Damage(fistObject, currentChargeTime);
					}
					currentState = FistState.Recovery;
				}
			}
		} else if (currentState == FistState.Stagger) {     //Stagger
			currentTimer += Time.deltaTime;
			if (currentTimer > staggerTime) {
				currentTimer = 0;
				currentState = FistState.Recovery;
			}

		} else if (currentState == FistState.Recovery) {    //Recovery
			MoveTo(fistStatePos[0].position, fistStatePos[1].position, fistObject, recoveryTime);
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > recoveryTime) {
				currentTimer = 0;
				currentState = FistState.Resting;
			}
		}
	}
	
	void MoveTo (Vector3 posL, Vector3 posR, GameObject fist, float timeBase) {
		Vector3 fistPos = fist.transform.position;
		Vector3 pos = Vector3.zero;
		if (fist.name == "LeftFist") {
			pos = posL;
		} else {
			pos = posR;
		}
		fist.transform.position += ((pos - fistPos).normalized * (pos - fistPos).magnitude / timeBase) * Time.deltaTime;
	}

	void Dodge (ref float currentTimer, float otherTimer, KeyCode currentKey, ref FistState currentState) {
		if (currentTimer < otherTimer) {
			if (currentKey == leftInput) {
				leftDodge = true;
				//RotatePivot(currentKey, currentState);
				MoveTo(fistStatePos[8].position, fistStatePos[9].position, leftFist, dodgeTime / 4);
				MoveTo(fistStatePos[8].position, fistStatePos[9].position, rightFist, dodgeTime / 4);

			} else if (currentKey == rightInput) {
				rightDodge = true;
				//RotatePivot(currentKey, currentState);
				MoveTo(fistStatePos[10].position, fistStatePos[11].position, leftFist, dodgeTime / 4);
				MoveTo(fistStatePos[10].position, fistStatePos[11].position, rightFist, dodgeTime / 4);
			}
		}
	}
	void RotatePivot (KeyCode currentKey, FistState current) {
		if (currentKey == leftInput) {
			pivot.transform.rotation *= Quaternion.Euler(0, 0, 1 * dodgeTime);
		} else {
			pivot.transform.rotation *= Quaternion.Euler(0, 0, -1 * dodgeTime);
		}
	}
	void Damage (GameObject fist, float chargeTime) {
		opponentHealth = opponent.GetComponent<PlayerScript>().health;
		opponentHealth -= Mathf.Clamp(punchDamage * (1 + chargeTime), 0, opponentHealth);
		opponent.GetComponent<PlayerScript>().leftFistState = FistState.Stagger;
		opponent.GetComponent<PlayerScript>().rightFistState = FistState.Stagger;
		opponent.GetComponent<PlayerScript>().health = opponentHealth;

		//Spawns effect when you hit
		GameObject hitIns = (GameObject)Instantiate(hitEffect, fist.transform.position, new Quaternion(0, 0, 0, 0));
		//Only shows for the player who hit
		if (tag == "Player 1") {
			hitIns.layer = 12;	//Player1Inv
		} else {
			hitIns.layer = 13;	//Player2Inv
		}
		Destroy(hitIns, 2);		//HARD CODE (destroys after 2sec)
	}
}
