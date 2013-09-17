using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public Transform target;
	public Vector3 offset;
	private float time;
	private Vector3 desired;
	
	void Update () {
		Vector3 newDesired = target.position;
		if (desired != newDesired) {
			time = 0;
			desired = newDesired;
		}
		time += Time.deltaTime*5;
		Vector3 lerpTarget = Vector3.Lerp(transform.position - offset, desired, time);
		transform.position = lerpTarget + offset;
		transform.LookAt(lerpTarget);
	}
	
	public void SetTarget(Transform t) {
		enabled = true;
		target = t;
	}
}
