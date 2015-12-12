using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public GameObject rightFist;
	public GameObject leftFist;
	public float inputDelay;
	public float startupTime;
	public float recoveryTime;
	public float blockTime;
	enum FistState { Resting, Input, Block, Startup, Recovery };
	FistState leftFistState;
	FistState rightFistState;
	float leftTimer;
	float rightTimer;

	void Update () {
		UpdateFist(ref leftFistState, ref leftTimer, rightFistState, ref rightTimer, KeyCode.A);	//LeftUpdate
		UpdateFist(ref rightFistState, ref rightTimer, leftFistState, ref leftTimer , KeyCode.D);	//RightUpdate
	}

	void UpdateFist (ref FistState currentState, ref float currentTimer, FistState other, ref float otherTimer, KeyCode currentKey) {

		if (currentState == FistState.Resting) {			//Resting
			Debug.Log(currentKey + ": " + currentState);
			if (Input.GetKeyDown(currentKey)) {
				currentState = FistState.Input;
				currentTimer = 0;
			}
		} else if (currentState == FistState.Input) {		//Input
			Debug.Log(currentKey + ": " + currentState);
			if (Input.GetKey(currentKey)) {
				currentTimer += Time.deltaTime;
				if (currentTimer < inputDelay && other == FistState.Input && otherTimer < inputDelay || other == FistState.Block) {
					currentState = FistState.Block;
					currentTimer = 0;
				}
			} else if (currentState != FistState.Block && other != FistState.Input && other != FistState.Startup) {
				currentTimer = 0;
				currentState = FistState.Startup;
			} else {
				currentTimer = 0;
				currentState = FistState.Resting;
			}
		} else if (currentState == FistState.Block) {		//Block
			Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > blockTime) {
				currentTimer = 0;
				currentState = FistState.Recovery;
			}

		} else if (currentState == FistState.Startup) {		//Startup
			Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > startupTime) {
				currentTimer = 0;
				currentState = FistState.Recovery;
			}
		} else if (currentState == FistState.Recovery) {	//Recovery
			Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > recoveryTime) {
				currentTimer = 0;
				currentState = FistState.Resting;
			}
		}
	}

	void Punch (GameObject fist) {
	
	}

	void Block () {

	}
}
