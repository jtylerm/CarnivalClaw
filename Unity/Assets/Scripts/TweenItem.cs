using System;
using UnityEngine;

public enum TweenItemType {
	Move,
	Rotate,
	Scale
}

public class TweenItem {

	public delegate void TweenMoveCallbackDelegate(GameObject gameObject);

	public TweenItemType type;
	public GameObject gameObject = null;

	public double duration = 0;

	public Vector3 startPosition;
	public Vector3 targetPosition;

	public Quaternion startRotation;
	public Quaternion targetRotation;

	public Vector3 startScale;
	public Vector3 targetScale;

	public TweenMoveCallbackDelegate tweenMoveCallbackDelegate;

	public bool HasReachedTargetPosition() {
		float totalDist = Vector3.Distance(startPosition, targetPosition);

		float distFromStart = Vector3.Distance(gameObject.transform.position, startPosition);

		return (distFromStart >= totalDist);
	}

	public bool HasReachedTargetRotation() {
		float totalAngle = Quaternion.Angle(startRotation, targetRotation);

		float degreesFromStart = Quaternion.Angle(gameObject.transform.rotation, startRotation);

		return (degreesFromStart >= totalAngle);
	}

	public void SnapToTargetPosition() {
		gameObject.transform.position = targetPosition;
	}

	public void SnapToTargetRotation() {
		gameObject.transform.rotation = targetRotation;
	}

	public void MoveTowardsTarget() {
		float totalDist = Vector3.Distance(startPosition, targetPosition);
		float distancePerSecond = (float)(totalDist/duration);

		float step = distancePerSecond * Time.deltaTime;
		gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, step);
	}

	public void RotateTowardsTarget() {
		float totalAngle = Quaternion.Angle(startRotation, targetRotation);
		float degreesPerSecond = (float)(totalAngle/duration);

		float step = degreesPerSecond * Time.deltaTime;
		gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, targetRotation, step);
	}
}

