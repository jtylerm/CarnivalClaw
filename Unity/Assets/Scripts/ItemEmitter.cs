﻿using System.Collections;
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

		GameObject itemGameObject = GameObject.Instantiate(this.templateGameObjects[randomIndex], targetTransform.parent);
		itemGameObject.transform.localPosition = this.transform.localPosition;

//		itemGameObject.transform.up = targetTransform.up;
//		itemGameObject.transform.forward = targetTransform.forward;
		//itemGameObject.transform.up = this.transform.worldToLocalMatrix.MultiplyVector(targetTransform.up);

//		Quaternion itemRotation = itemGameObject.transform.localRotation;
//		Debug.Log("item rotation: " + itemRotation);
//		Vector3 itemEuler = itemRotation.eulerAngles;
//		Vector3 euler = this.transform.eulerAngles;
//		itemRotation = Quaternion.Euler(itemEuler.x + euler.x, itemEuler.y + euler.y, itemEuler.z + euler.z);
//		Debug.Log("new item rotation: " + itemRotation);
//		itemGameObject.transform.localRotation = itemRotation;

		itemGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

//		Vector3 targetScale = targetTransform.lossyScale;
//		Vector3 itemLocalScale = itemGameObject.transform.localScale;
//		Debug.Log("scale: " + targetScale + ", itemLocalScale: " + itemLocalScale);
//		itemLocalScale = Vector3.Scale(targetScale, itemLocalScale);
//		Debug.Log("new itemLocalScale: " + itemLocalScale);
//		//itemGameObject.transform.localScale = itemLocalScale;

		//itemGameObject.transform.parent = targetTransform.parent;
		Item item = itemGameObject.GetComponent<Item>();
		item.targetTransform = this.targetTransform;
	}

}
