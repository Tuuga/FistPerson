using UnityEngine;
using System.Collections;

public class ObjectScript : MonoBehaviour {

	public float damageModifier;
	GameObject conv_1;
	GameObject conv_2;

	void Awake () {
		conv_1 = GameObject.FindGameObjectWithTag("Conv 1");
		conv_2 = GameObject.FindGameObjectWithTag("Conv 2");
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == "Dropper") {
			conv_1.GetComponent<ConveyorScript>().DropObject(gameObject);
			conv_2.GetComponent<ConveyorScript>().DropObject(gameObject);
			Destroy(gameObject, 2f);
		} else if (c.tag == "Destroyer") {
			Destroy(gameObject);
		}
	}
}
