﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

	public KeyCode leftInput;
	public KeyCode rightInput;
	KeyCode lastKey;

	public GameObject hp;
	public GameObject leftConv;
	public GameObject rightConv;
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
	//Lists for fists
	List<GameObject> leftFistObjects;
	List<GameObject> rightFistObjects;
	float maxHP;

	void Start () {
		maxHP = health;
		leftFistObjects = new List<GameObject>();
		rightFistObjects = new List<GameObject>();

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

		Vector3 hpScale = hp.transform.localScale;
		hpScale = new Vector3(health / maxHP, 1, 1);
		hp.transform.localScale = hpScale;

		UpdateFist(ref leftFistState, ref leftTimer, rightFistState, ref rightTimer, leftInput, leftFist);      //LeftUpdate
		UpdateFist(ref rightFistState, ref rightTimer, leftFistState, ref leftTimer, rightInput, rightFist);    //RightUpdate
	}

	void UpdateFist (ref FistState currentState, ref float currentTimer, FistState other, ref float otherTimer, KeyCode currentKey, GameObject fistObject) {

		if (currentState == FistState.Resting) {            //Resting
			MoveTo(fistStatePos[0].position, fistStatePos[1].position, fistObject, recoveryTime);
			if (showStateLogs)
				Debug.Log(currentKey + ": " + currentState);
			if (Input.GetKeyDown(currentKey)) {
				lastKey = currentKey;
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
			if (!conveyorMode) {
				MoveTo(fistStatePos[2].position, fistStatePos[3].position, fistObject, startupTime);
			} else {
				MoveTo(fistStatePos[12].position, fistStatePos[13].position, fistObject, startupTime);
			}
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
					if (!conveyorMode) {
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
		float dm = 0;
		opponentHealth = opponent.GetComponent<PlayerScript>().health;
		opponent.GetComponent<PlayerScript>().leftFistState = FistState.Stagger;
		opponent.GetComponent<PlayerScript>().rightFistState = FistState.Stagger;

		if (fist == leftFist && leftFistObjects.Count > 0) {
			if (leftFistObjects[0] != null) {
				dm = leftFistObjects[0].GetComponent<ObjectScript>().damageModifier;
				leftFistObjects[0].GetComponentInChildren<MeshCollider>().enabled = true;
				leftFistObjects[0].transform.SetParent(null);
				leftFistObjects[0].GetComponent<Rigidbody>().isKinematic = false;
				leftFistObjects[0].GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * 100f);
				Destroy(leftFistObjects[0], 3f);
            }
			leftFistObjects.Remove(leftFistObjects[0]);

		} else if (rightFistObjects.Count > 0) {
			if (rightFistObjects[0] != null) {
				dm = rightFistObjects[0].GetComponent<ObjectScript>().damageModifier;
				rightFistObjects[0].GetComponentInChildren<MeshCollider>().enabled = true;
				rightFistObjects[0].transform.SetParent(null);
				rightFistObjects[0].GetComponent<Rigidbody>().isKinematic = false;
				rightFistObjects[0].GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * 100f);
				Destroy(rightFistObjects[0], 3f);
			}
			rightFistObjects.Remove(rightFistObjects[0]);
		}
		opponentHealth -= Mathf.Clamp((punchDamage + dm) * (1 + chargeTime), 0, opponentHealth);
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

	void AddObject (GameObject g) {
		g.GetComponent<Rigidbody>().isKinematic = true;
		g.transform.GetComponentInChildren<MeshCollider>().enabled = false;
        if (lastKey == leftInput) {
			//attach to left fist
			g.transform.SetParent(leftFist.transform);
			leftFistObjects.Add(g);
			leftConv.GetComponent<ConveyorScript>().allObjects.Remove(g);
			g.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
		} else {
			//attach to right fist
			g.transform.SetParent(rightFist.transform);
			rightFistObjects.Add(g);
			rightConv.GetComponent<ConveyorScript>().allObjects.Remove(g);
			g.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
		}
	}
	void OnTriggerEnter (Collider c) {
		if (c.transform.parent != null && c.transform.parent.tag == "Object" && conveyorMode == true) {
			AddObject(c.transform.parent.gameObject);
		}
	}
}
