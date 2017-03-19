using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenHelper : MonoBehaviour {

	public static TweenHelper defaultTweenHelper = null;

	List<TweenItem> tweenItems = new List<TweenItem>();

	void Awake() {
		defaultTweenHelper = this;
	}

	void Start() {

	}

	void Update() {
		List<TweenItem> tempTweenItems = new List<TweenItem>(tweenItems);

		foreach(TweenItem tweenItem in tempTweenItems) {
			if(tweenItem.type == TweenItemType.Move) {
				if(tweenItem.HasReachedTargetPosition()) {
					tweenItem.SnapToTargetPosition();

					tweenItems.Remove(tweenItem);
					tweenItem.tweenMoveCallbackDelegate(tweenItem.gameObject);
				}
				else {
					tweenItem.MoveTowardsTarget();
				}
			}
			else if(tweenItem.type == TweenItemType.MoveLocal) {
				if(tweenItem.HasReachedTargetPositionLocal()) {
					tweenItem.SnapToTargetPositionLocal();

					tweenItems.Remove(tweenItem);
					tweenItem.tweenMoveCallbackDelegate(tweenItem.gameObject);
				}
				else {
					tweenItem.MoveTowardsTargetLocal();
				}
			}
			else if(tweenItem.type == TweenItemType.Rotate) {
				if(tweenItem.HasReachedTargetRotation()) {
					tweenItem.SnapToTargetRotation();

					tweenItems.Remove(tweenItem);
				}
				else {
					tweenItem.RotateTowardsTarget();
				}
			}
		}
	}

	public void TweenMove(GameObject gameObject, double duration, Vector3 targetPosition, TweenItem.TweenMoveCallbackDelegate callbackDelegate) {
		TweenItem tweenItem = new TweenItem();
		tweenItem.type = TweenItemType.Move;
		tweenItem.gameObject = gameObject;
		tweenItem.duration = duration;
		tweenItem.startPosition = gameObject.transform.position;
		tweenItem.targetPosition = targetPosition;
		tweenItem.tweenMoveCallbackDelegate = callbackDelegate;

		tweenItems.Add(tweenItem);
	}

	public void TweenMoveLocal(GameObject gameObject, double duration, Vector3 localTargetPosition, TweenItem.TweenMoveCallbackDelegate callbackDelegate) {
		TweenItem tweenItem = new TweenItem();
		tweenItem.type = TweenItemType.MoveLocal;
		tweenItem.gameObject = gameObject;
		tweenItem.duration = duration;
		tweenItem.localStartPosition = gameObject.transform.localPosition;
		tweenItem.localTargetPosition = localTargetPosition;
		tweenItem.tweenMoveCallbackDelegate = callbackDelegate;

		tweenItems.Add(tweenItem);
	}

	public void TweenRotate(GameObject gameObject, double duration, Quaternion targetRotation) {
		TweenItem tweenItem = new TweenItem();
		tweenItem.type = TweenItemType.Rotate;
		tweenItem.gameObject = gameObject;
		tweenItem.duration = duration;
		tweenItem.startRotation = gameObject.transform.rotation;
		tweenItem.targetRotation = targetRotation;

		tweenItems.Add(tweenItem);
	}
}

