using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour {

	public Transform clawTopTarget;
	public Transform clawBottomTarget;
	public bool isReady = true;

	Item item;

	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider otherCollider) {
		item = otherCollider.gameObject.GetComponent<Item>();

		if(item != null) {
			item.shouldAnimate = false;

			item.transform.parent = this.transform;
		}
	}

	public void DestroyChildItem() {
		if(item != null) {
			Destroy(item.gameObject);
		}
	}

	public int ChangeScore () {
		if(item != null) {
			return item.points;
		}
		else {
			return 0;
		}
	}
}
