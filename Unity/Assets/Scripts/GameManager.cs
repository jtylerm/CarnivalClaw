using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public static GameManager defaultGameManager = null;

	Claw claw;
	Vector3 clawStartPosition;

	Text scoreText;
	Text timerText;

	float time = 0;
	float seconds = 0;

	int score = 0;
	int scoreLastRound = 0;
	int itemPointWorth = 0;

	WebRequestManager webRequestManager = null;


	int playerID = -1;
	string playerUsername = null;



	GameState gameState = GameState.UNSPECIFIED;
	GameState nextGameState = GameState.UNSPECIFIED;
	float nextStateStartTime = 0;
	float reportedTimeRemaining = 0;
	string currentRoundID = null;

	float lastWebUpdateTime = 0;
	bool shouldFireWebUpdate = false;

	void Awake() {
		PlayerPrefs.DeleteAll();

		defaultGameManager = this;

		claw = GameObject.Find("Claw").GetComponent<Claw>();

		clawStartPosition = claw.gameObject.transform.position;

		scoreText = GameObject.Find("Canvas").transform.Find("ScoreText").GetComponent<Text>();
		timerText = GameObject.Find("Canvas").transform.Find("TimerText").GetComponent<Text>();

		webRequestManager = new WebRequestManager();
	}

	void Start() {

		if(PlayerPrefs.HasKey("playerID")){
			this.playerID = PlayerPrefs.GetInt("playerID");
			this.playerUsername = PlayerPrefs.GetString("playerUsername");

			//TODO: load user UI

			StartWebUpdateTimer();
		}
		else {
			StartCoroutine(webRequestManager.GetNewUser());
		}
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



			//Update the timer
			UpdateTimerUI();
		}

		if(shouldFireWebUpdate){

			if(Time.time - lastWebUpdateTime > 1){
				//Debug.Log("web update time diff: " + (Time.time - lastWebUpdateTime));
				lastWebUpdateTime = Time.time;
				//TODO: get index of selected target image
				int orientationImageIndex = 0;

				StartCoroutine(webRequestManager.SendPlayerUpdate(orientationImageIndex, this.playerID, this.score, this.currentRoundID));
			}
		}

	}

	void StartWebUpdateTimer(){
		shouldFireWebUpdate = true;
	}
		
	public void DidGetNewUser(int id, string username){
		this.playerID = id;
		this.playerUsername = username;

		PlayerPrefs.SetInt("playerID", this.playerID);
		PlayerPrefs.SetString("playerUsername", this.playerUsername);

		//TODO: load user UI

		StartWebUpdateTimer();
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
				UpdateScoreUI();
			}
			else if(gameState == GameState.IN_ROUND) {

			}

			UI_Manager.defaultUI_Manager.UpdateForGameState(gameState, playerUsername, scoreLastRound);

			//TODO: change UI
		}

		this.nextStateStartTime = Time.time + timeRemaining;
		this.reportedTimeRemaining = timeRemaining;
	}

	public void FireClawTop() {
		if(claw.isReady == true) {
			claw.isReady = false;
			TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, claw.clawTopTarget.transform.position, DidFinishAnimatingClawToTarget);
		}
	}

	public void FireClawBottom() {
		if(claw.isReady == true) {
			claw.isReady = false;
			TweenHelper.defaultTweenHelper.TweenMove(claw.gameObject, .25f, claw.clawBottomTarget.transform.position, DidFinishAnimatingClawToTarget);
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
		UpdateScoreUI();

		//Reset claw's isReady status
		claw.isReady = true;
	}

	void UpdateTimerUI() {
		timerText.text = "Time " + string.Format(":{0:00}", reportedTimeRemaining);
	}

	void UpdateScoreUI() {
		scoreText.text = "Score: " + score.ToString();
	}

	public void TempSimPlayerScoreIncrease(){
		score += 1;
		UpdateScoreUI();
	}
}