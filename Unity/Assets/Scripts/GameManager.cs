using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	Claw claw;
	Vector3 clawStartPosition;

	Text scoreText;
	Text timerText;

	float time = 0;
	float seconds = 0;

	int score = 0;
	int itemPointWorth = 0;

	bool roundHasEnded = false;

	void Awake() {
		claw = GameObject.Find("Claw").GetComponent<Claw>();

		clawStartPosition = claw.gameObject.transform.position;

		scoreText = GameObject.Find("Canvas").transform.Find("ScoreText").GetComponent<Text>();
		timerText = GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>();
	}

	void Update() {
		if(roundHasEnded == false) {
			if(Input.GetMouseButtonDown(0) && claw.isReady) {
				claw.isReady = false;
				TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, claw.clawTopTarget.transform.position, DidFinishAnimatingClawToTarget);
			}
			else if(Input.GetMouseButtonDown(1) && claw.isReady) {
				claw.isReady = false;
				TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, claw.clawBottomTarget.transform.position, DidFinishAnimatingClawToTarget);
			}

			//Update the timer
			UpdateTimerUI();
		}
	}

	void DidFinishAnimatingClawToTarget(GameObject gameObject) {
		TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, clawStartPosition, DidFinishAnimatingClawToStart);
	}

	void DidFinishAnimatingClawToStart(GameObject gameObject) {

		//Destroy child game object from scene
		claw.DestroyChildItem();

		//Update score information
		itemPointWorth = claw.ChangeScore();
		score += itemPointWorth;
		scoreText.text = "Score: " + score.ToString();

		//Reset claw's isReady status
		claw.isReady = true;
	}

	void UpdateTimerUI() {
		time += Time.deltaTime;

		if(seconds < 30) {
			seconds = time % 60;
			timerText.text = "Time " + string.Format(":{0:00}", seconds);
		}
		else {
			RoundHasEnded();
		}
	}

	void RoundHasEnded() {
		roundHasEnded = true;
	}
}