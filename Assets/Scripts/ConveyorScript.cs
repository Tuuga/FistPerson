using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyorScript : MonoBehaviour {

	public GameObject[] objects;
	[HideInInspector]
	public List<GameObject> allObjects;
	public GameObject spawnPoint;
	public GameObject trashCan;
	public float spawnTime;
	public float objectSpeed;
	public float timer;
	public bool convOn = true;
	Vector3 trashCanPos;

	void Start () {
		allObjects = new List<GameObject>();
		trashCanPos = trashCan.transform.position;
	}
	
	void Update () {
		MoveObjects();
		timer += Time.deltaTime;
		if (timer > spawnTime && convOn) {
			timer = 0;
			SpawnObject();
		}
	}
	void SpawnObject () {
		int randInd = Random.Range(0, objects.Length);
		float randRot = Random.Range(0, 360);
		GameObject objIns = (GameObject)Instantiate(objects[randInd], spawnPoint.transform.position, Quaternion.Euler(0, randRot, 0));
		allObjects.Add(objIns);
	}
	void MoveObjects() {
		for (int i = 0; i < allObjects.Count; i++) {
			Vector3 pos = allObjects[i].transform.position;
			allObjects[i].transform.position += (trashCanPos - pos).normalized * Time.deltaTime * objectSpeed;
			if ((trashCanPos - pos).magnitude < 0.1f) {
				Destroy(allObjects[i]);
				allObjects.Remove(allObjects[i]);
            }
		}
	}
}
