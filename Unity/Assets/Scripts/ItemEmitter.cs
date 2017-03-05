using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEmitter : MonoBehaviour {

	public Transform targetTransform;
	public GameObject[] templateGameObjects;
	private double lastEmitTime = 0;
	private double WAIT_DURATION = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//spacing out the item emissions
		if(Time.time - lastEmitTime >= WAIT_DURATION) {
			EmitItem();
			lastEmitTime = Time.time;
		}
	}

	void EmitItem() {

		int randomIndex = Random.Range(0,4);

		GameObject itemGameObject = GameObject.Instantiate(this.templateGameObjects[randomIndex]);
		itemGameObject.transform.position = this.transform.position;
		itemGameObject.transform.up = targetTransform.up;
		itemGameObject.transform.parent = targetTransform.parent;
		Item item = itemGameObject.GetComponent<Item>();
		item.targetTransform = this.targetTransform;
	}
}
