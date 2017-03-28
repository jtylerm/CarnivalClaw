using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour {

	public GameObject orientationObj;
	public GameObject gravityObj;

	private List<TransformRecord> transformRecords = new List<TransformRecord>();

	private float TIME_WINDOW = 1;

	private float scaleWithParent = 1;

	private float scaleMultiplier = 1;
	private float positionZOffset = 0;

	void Awake() {

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//if(this.transform.parent == null && orientationObj != null){
		if(this.transform.parent == null 
			&& this.orientationObj != null)
		{
			this.transform.parent = orientationObj.transform;

			this.scaleWithParent = this.transform.localScale.x;

			UpdateTransform();
		}

		if(this.orientationObj != null){


		}

		/*
		//this.transform.position = orientationObj.transform.position;
		Quaternion orientationRotation = orientationObj.transform.rotation;

		Vector3 objEulerRot = orientationObj.transform.rotation.eulerAngles;
		if(objEulerRot.x == 0 && objEulerRot.y == 0 && objEulerRot.z == 0)
		{
			//if orientation object not rotated, it's likely not being recognized
			return;
		}


		TransformRecord transformRecord = new TransformRecord();
		transformRecord.position = orientationObj.transform.position;
		transformRecord.rotation = orientationRotation;
		transformRecord.timeStamp = Time.time;
		transformRecords.Add(transformRecord);

		float currentTime = Time.time;

		//purge older rotation times
		for(int i = transformRecords.Count - 1; i >= 0; i--) { 
			//if rotation time is more than TIME_WINDOW units away from current time, remove that object from list
			if (transformRecords[i].timeStamp < (currentTime - TIME_WINDOW)) {
				transformRecords.RemoveAt(i);
			}
		}

		Vector3 positionSumVector = Vector3.zero;
		Vector3 rotationSumVector = Vector3.zero;

		//loop through list to calculate average rotation
		for(int i = 0; i < transformRecords.Count; i++) {
			positionSumVector.x += transformRecords[i].position.x;
			positionSumVector.y += transformRecords[i].position.y;
			positionSumVector.z += transformRecords[i].position.z;

			rotationSumVector.x += NormalizeAngle(transformRecords[i].rotation.eulerAngles.x);
			rotationSumVector.y += NormalizeAngle(transformRecords[i].rotation.eulerAngles.y);
			rotationSumVector.z += NormalizeAngle(transformRecords[i].rotation.eulerAngles.z);
		}

		Vector3 averagePosition = new Vector3(positionSumVector.x / transformRecords.Count, positionSumVector.y / transformRecords.Count, positionSumVector.z / transformRecords.Count);
		Quaternion averageRotation = Quaternion.Euler(rotationSumVector.x / transformRecords.Count, rotationSumVector.y / transformRecords.Count, rotationSumVector.z / transformRecords.Count);
		//Quaternion targetRotation = Quaternion.Euler(Input.gyro.attitude.eulerAngles.x, rotationAverage.eulerAngles.y, Input.gyro.attitude.eulerAngles.z);
		//Quaternion targetRotation = rotationAverage.look;

		gravityObj.transform.rotation = Input.gyro.attitude;

		Debug.Log("gyro.attitude: " + Input.gyro.attitude.eulerAngles);
		Debug.Log("rotationAverage: " + averageRotation.eulerAngles);

		this.transform.position = averagePosition;

		//this.transform.rotation = averageRotation;

		//Quaternion relativeRotation = Quaternion.Inverse(Input.gyro.attitude) * orientationRotation;
		Quaternion relativeRotation = Quaternion.Inverse(Input.gyro.attitude) * averageRotation;

		Debug.Log("relativeRotation: " + relativeRotation.eulerAngles);

		float isolatedRotationY = (relativeRotation.eulerAngles.y > 180) ? relativeRotation.eulerAngles.y: relativeRotation.eulerAngles.y;
		isolatedRotationY = NormalizeAngle(isolatedRotationY);

		Debug.Log("isolatedRotationY: " + isolatedRotationY);

		Quaternion isolatedRotation = Quaternion.Euler(0, isolatedRotationY, 0);

		Debug.Log("isolatedRotation: " + isolatedRotation.eulerAngles);

		Quaternion targetRotation;

		if(Application.isMobilePlatform){
			Debug.Log("Mobile Platform");
			targetRotation = Input.gyro.attitude * isolatedRotation * Quaternion.Euler(-90, 0, 0);
		}
		else {
			Debug.Log("Other Platform");
			targetRotation = Input.gyro.attitude * isolatedRotation * Quaternion.Euler(-90, 0, 0);
		}

		//Vector3 lerpedAngle = AngleLerp(this.transform.rotation.eulerAngles, targetRotation.eulerAngles, Time.deltaTime);
		//targetRotation = Quaternion.Euler(lerpedAngle);
		this.transform.rotation = targetRotation;

//		Vector3 temp = -(Input.acceleration.normalized);
//		temp.y = 0; // rotate around x axis only
//		transform.up = temp.normalized;

		//this.transform.up = -(Input.acceleration.normalized);

		//this.transform.rotation = Quaternion.Euler(Input.gyro.attitude.eulerAngles.x, averageRotation.eulerAngles.y, averageRotation.eulerAngles.z);

		//this.transform.up = gravityObj.transform.up;

		//Align(gravityObj.transform.forward, gravityObj.transform.right);

		//this.transform.rotation *= Quaternion.Euler(Vector3.right * -90);
		*/

	}

	void UpdateTransform(){
		// SCALE 
		float scale = this.scaleWithParent * this.scaleMultiplier; 
		this.transform.localScale = new Vector3(scale, scale, scale);

		//POSITION
		this.transform.localPosition = new Vector3(0, 0, this.positionZOffset);

		//ROTATION
		this.transform.localRotation = Quaternion.identity;
	}

	public void setScalePositionOverrides(float scaleMultiplier, float positionZOffset){
		this.scaleMultiplier = scaleMultiplier;
		this.positionZOffset = positionZOffset;

		UpdateTransform();
	}

	float NormalizeAngle(float angle) {
		while(angle < -180){
			angle += 360;
		}

		while(angle >= 180){
			angle -= 360;
		}
		
		return angle;
	}

	Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
	{        
		float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
		float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
		float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
		Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
		return Lerped;
	}

	void Align(Vector3 alignY, Vector3 alignZ) {
		//Align Z first
		transform.rotation = Quaternion.FromToRotation(Vector3.forward, alignZ);

		//Get component of alignY that is in the plane YZ
		alignY -= Vector3.Dot(alignY, transform.right) * transform.right;

		//Get the angle needed to align Y
		float angle = Vector3.Angle(transform.up, alignY);

		//Fix angle sign
		if (Vector3.Dot(Vector3.Cross(alignY, transform.up), transform.right) > 0.0f) {
			angle *= -1.0f;
		}

		//For some reason this wasn't working
		//transform.rotation *= Quaternion.AngleAxis(angle, transform.right);

		//Align Y
		transform.Rotate(transform.right, angle, Space.World);
	}
}
