using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public GameObject rightFist;
	public GameObject leftFist;
	public float chargeMaxTime;
	public float inputDelayBlock;
	public float startupTime;
	public float recoveryTime;
	public float blockTime;
	public bool showStateLogs;
	enum FistState { Resting, Input, Block, Startup, Recovery };
	FistState leftFistState;
	FistState rightFistState;
	float leftTimer;
	float rightTimer;
	public Transform[] fistStatePos;


	void Update () {
		if (tag == "Player 1") {
			UpdateFist(ref leftFistState, ref leftTimer, rightFistState, ref rightTimer, KeyCode.A, leftFist);      //LeftUpdate
			UpdateFist(ref rightFistState, ref rightTimer, leftFistState, ref leftTimer, KeyCode.D, rightFist);    //RightUpdate
		} else {
			UpdateFist(ref leftFistState, ref leftTimer, rightFistState, ref rightTimer, KeyCode.LeftArrow, leftFist);      //LeftUpdate
			UpdateFist(ref rightFistState, ref rightTimer, leftFistState, ref leftTimer, KeyCode.RightArrow, rightFist);    //RightUpdate
		}

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
				if (currentTimer < inputDelayBlock && other == FistState.Input && otherTimer < inputDelayBlock || other == FistState.Block) {
					currentState = FistState.Block;
					currentTimer = 0;
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

		} else if (currentState == FistState.Startup) {     //Startup
			MoveTo(fistStatePos[2].position, fistStatePos[3].position, fistObject, startupTime);
			if (showStateLogs)
			Debug.Log(currentKey + ": " + currentState);
			currentTimer += Time.deltaTime;
			if (currentTimer > startupTime) {
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
}
