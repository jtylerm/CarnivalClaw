using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackableImageTarget : MonoBehaviour, ITrackableEventHandler {

	TrackableBehaviour trackableBehaviour = null;

	public int index;

	// Use this for initialization
	void Start () {
		RegisterTrackableBehavior();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RegisterTrackableBehavior(){
		trackableBehaviour = GetComponent<TrackableBehaviour>();
		if (trackableBehaviour != null)
		{
			trackableBehaviour.RegisterTrackableEventHandler(this);

		}
	}

	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			GameManager.defaultGameManager.DidRecognizeImageTarget(this);
		}
		else
		{
			//show message "you must be pointed at the screen"
			GameManager.defaultGameManager.DidLoseImageTarget(this);
		}
	} 
}
