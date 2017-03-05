using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public Transform targetTransform;
	float speed = 16;

	public int points = 0;

	public bool shouldAnimate = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldAnimate) {
			MoveTowardTarget();
		}
	}

	public void MoveTowardTarget() {
		if(this.transform.localPosition == targetTransform.localPosition) {
			Destroy(this.gameObject);
		}
		else {
			float step = speed * Time.deltaTime;
			this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetTransform.localPosition, step);
		}
	}
}
