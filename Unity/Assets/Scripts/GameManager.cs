﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using mixpanel;

public class GameManager : MonoBehaviour {
	
	public static GameManager defaultGameManager = null;

	WebRequestManager webRequestManager = null;

	//private List<TrackableImageTarget> trackableImageTargets = new List<TrackableImageTarget>();

	Claw claw;
	Vector3 clawStartPosition;

	Stage stage = null;

	int score = 0;
	int scoreLastRound = 0;
	int itemPointWorth = 0;

	bool hasPlayer = false;
	int playerID = -1;
	string playerUsername = null;

	GameState gameState = GameState.UNSPECIFIED;
	GameState nextGameState = GameState.UNSPECIFIED;
	float nextStateStartTime = 0;
	float reportedTimeRemaining = 0;
	string currentRoundID = null;

	TrackableImageTarget activeImageTarget = null;

	float lastWebUpdateTime = 0;

	void Awake() {
		//PlayerPrefs.DeleteAll();

		defaultGameManager = this;

		claw = GameObject.Find("Claw").GetComponent<Claw>();

		stage = GameObject.Find("Stage").GetComponent<Stage>();

		webRequestManager = new WebRequestManager();
	}

	void Start() {

		Mixpanel.Track("App.Launched");

		if(PlayerPrefs.HasKey("playerID")){
			
			this.playerID = PlayerPrefs.GetInt("playerID");
			this.playerUsername = PlayerPrefs.GetString("playerUsername");

			Mixpanel.Register("UserID", this.playerID);
			Mixpanel.Track("App.DidLoadExistingUser");

			UI_Manager.defaultUI_Manager.UpdateForGameState(GameState.WAITING_NEXT_ROUND, playerUsername, scoreLastRound);

			this.hasPlayer = true;
		}
		else {
			//no local player data
			UI_Manager.defaultUI_Manager.ShowNewUserPanel();
		}

		claw.isReady = true;

	}

	void Update() {
		if(gameState == GameState.IN_ROUND) {

			//keyboard commands for when not building mobile
			#if !UNITY_ANDROID && !UNITY_IOS
			if(Input.GetMouseButtonDown(0) && claw.isReady) {
				claw.isReady = false;
				TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, claw.clawTopTarget.transform.position, DidFinishAnimatingClawToTarget);
			}
			else if(Input.GetMouseButtonDown(1) && claw.isReady) {
				claw.isReady = false;
				TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, claw.clawBottomTarget.transform.position, DidFinishAnimatingClawToTarget);
			}
			#endif

			if(activeImageTarget != null) {
				UI_Manager.defaultUI_Manager.SetARWarningPanelVisible(false);
			}
			else {
				UI_Manager.defaultUI_Manager.SetARWarningPanelVisible(true);
			}
		}

		if(this.hasPlayer && activeImageTarget != null){

			if(Time.time - lastWebUpdateTime > 1){
				//Debug.Log("web update time diff: " + (Time.time - lastWebUpdateTime));
				lastWebUpdateTime = Time.time;

				//get index of selected target image
				int orientationImageIndex = (activeImageTarget != null) ? activeImageTarget.index - 1 : -1;

				

				StartCoroutine(webRequestManager.SendPlayerUpdate(orientationImageIndex, this.playerID, this.score, this.currentRoundID));
			}
		}

	}
		
	public void DidSelectGetNewUser(){
		Mixpanel.Track("App.DidSelectGetNewUser");
		StartCoroutine(webRequestManager.GetNewUser());
	}

	public void DidGetNewUser(int id, string username){
		this.playerID = id;
		this.playerUsername = username;

		Mixpanel.Register("UserID", this.playerID);

		PlayerPrefs.SetInt("playerID", this.playerID);
		PlayerPrefs.SetString("playerUsername", this.playerUsername);

		//hide new user UI
		UI_Manager.defaultUI_Manager.HideNewUserPanel();

		this.hasPlayer = true;

		Mixpanel.Track("App.DidGetNewUser");
	}

	public void DidGetGameUpdate(GameState gameState, GameState nextGameState, int timeRemaining, string currentRoundID){

		if(this.gameState != gameState 
			|| this.currentRoundID != currentRoundID){

			this.gameState = gameState;
			this.nextGameState = nextGameState;
			this.currentRoundID = currentRoundID;

			if(gameState == GameState.WAITING_NEXT_ROUND) {
				scoreLastRound = score;
				score = 0;
			}
			else if(gameState == GameState.IN_ROUND) {

			}

			Value props = new Value();
			props["gameState"] = new Value(this.gameState.ToString());
			Mixpanel.Track("App.DidGetGameUpdate.GameStateChanged", props);

			UI_Manager.defaultUI_Manager.UpdateForGameState(gameState, playerUsername, scoreLastRound);
		}

		this.nextStateStartTime = Time.time + timeRemaining;
		this.reportedTimeRemaining = timeRemaining;
	}

	public void FireClawTop() {
		if(claw.isReady == true) {
			claw.isReady = false;
			clawStartPosition = claw.gameObject.transform.localPosition;
			TweenHelper.defaultTweenHelper.TweenMoveLocal(claw.gameObject, .25f, claw.clawTopTarget.transform.localPosition, DidFinishAnimatingClawToTarget);
		}
	}

	public void FireClawBottom() {
		if(claw.isReady == true) {
			claw.isReady = false;
			TweenHelper.defaultTweenHelper.TweenMoveLocal(claw.gameObject, .25f, claw.clawBottomTarget.transform.localPosition, DidFinishAnimatingClawToTarget);
		}
	}

	void DidFinishAnimatingClawToTarget(GameObject gameObject) {
		TweenHelper.defaultTweenHelper.TweenMoveLocal(claw.gameObject, .25f, clawStartPosition, DidFinishAnimatingClawToStart);
	}

	void DidFinishAnimatingClawToStart(GameObject gameObject) {

		if(gameState == GameState.IN_ROUND){
			//Update score information
			itemPointWorth = claw.GetPoints();
			score += itemPointWorth;
			UI_Manager.defaultUI_Manager.UpdateGameplayScore(score);
		}
			
		//Destroy child game object from scene
		claw.DestroyChildItems();

		//Reset claw's isReady status
		claw.isReady = true;
	}



//	public void TempSimPlayerScoreIncrease(){
//		score += 1;
//		UI_Manager.defaultUI_Manager.UpdateGameplayScore(score);
//	}

	//vuforia

//	void RegisterTrackableImageTargets(){
//		TrackableImageTarget trackableImageTarget = null;
//
//		GameObject arCamera = GameObject.Find("ARCamera");
//
//		for(int i = 0; i < 6; i++){
//			trackableImageTarget = arCamera.transform.Find("ImageTarget" + i).GetComponent<TrackableImageTarget>();
//			if (trackableImageTarget != null)
//			{
//				trackableImageTargets.Add(trackableImageTarget);
//			}
//		}
//	}

	public void DidRecognizeImageTarget(TrackableImageTarget imageTarget){
		activeImageTarget = imageTarget;
		stage.orientationObj = imageTarget.gameObject;

		Value props = new Value();
		props["imageTargetIndex"] = new Value(imageTarget.index.ToString());
		Mixpanel.Track("App.DidRecognizeImageTarget", props);
	}

	public void DidLoseImageTarget(TrackableImageTarget imageTarget){
		activeImageTarget = null;
		stage.orientationObj = null;

		Value props = new Value();
		props["imageTargetIndex"] = new Value(imageTarget.index.ToString());
		Mixpanel.Track("App.DidLoseImageTarget", props);
	}

	  
}