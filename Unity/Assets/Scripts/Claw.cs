using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour {

	public Transform clawTopTarget;
	public Transform clawBottomTarget;
	bool _isReady;

	public bool isReady
	{
		get
		{
			return _isReady;
		}
		set
		{
			_isReady = value;
		}
	}

	List<Item> items = new List<Item>();

	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider otherCollider) {
		Item item = otherCollider.gameObject.GetComponent<Item>();

		if(item != null) {
			item.shouldAnimate = false;

			item.transform.parent = this.transform;

			items.Add(item);
		}
	}

	public void DestroyChildItems() {
		if(items.Count > 0) {
			foreach(Item item in this.items){
				Destroy(item.gameObject);
			}
			items.Clear();
		}
	}

	public int GetPoints () {
		if(items.Count > 0) {
			int points = 0;
			foreach(Item item in this.items){
				points += item.points;
			}
			return points;
		}
		else {
			return 0;
		}
	}
}
