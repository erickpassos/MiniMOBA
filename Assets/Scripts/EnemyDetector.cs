using UnityEngine;
using System.Collections;

public class EnemyDetector : MonoBehaviour {

	void OnTriggerStay (Collider other) {
		if (Network.isServer && other.tag != transform.tag) {
			Character t = transform.parent.GetComponent<CreepAI>().target;
			if (t == null || t.health <= 0) {
				transform.parent.GetComponent<CreepAI>().target = other.GetComponent<Character>();
			}
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (Network.isServer && other.tag != transform.tag)
			transform.parent.GetComponent<CreepAI>().target = null;
	}
}
