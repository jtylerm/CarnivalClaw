using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public static UI_Manager defaultUI_Manager;

	public GameObject afterRoundInfoPanel;

	public GameObject normalAfterRoundInfoPanel;

	public GameObject recordBreakAfterRoundInfoPanel;

	public Text afterRoundScoreText;

	public GameObject newUserPanel;

	public GameObject gameplayUIPanel;

	public GameObject arWarningPanel;

	public Text usernameText;
	public Text gameplayScoreText;


	void Awake() {
		defaultUI_Manager = this;

//		afterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("AfterRoundInfoPanel").gameObject;
//		normalAfterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("NormalAfterRoundInfoPanel").gameObject;
//		normalAfterRoundInfoPanel.SetActive(true);
//		recordBreakAfterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("RecordBreakAfterRoundInfoPanel").gameObject;
//		usernameText = afterRoundInfoPanel.transform.Find("UsernameText").GetComponent<Text>();
//		afterRoundScoreText = afterRoundInfoPanel.transform.Find("ScoreText").GetComponent<Text>();
//
//		newUserPanel = GameObject.Find("Canvas").transform.Find("NewUserPanel").gameObject;
//
//		gameplayUIPanel = GameObject.Find("Canvas").transform.Find("GameplayUI_Panel").gameObject;
//		gameplayScoreText = gameplayUIPanel.transform.Find("ScoreText").GetComponent<Text>();
	}

	void Start () {
		
	}

	void Update () {
		
	}

	public void UpdateForGameState(GameState gameState, string username, int score) {
		if(gameState == GameState.IN_ROUND) {
			afterRoundInfoPanel.SetActive(false);
			gameplayUIPanel.SetActive(true);
		}
		else {
			afterRoundInfoPanel.SetActive(true);

			usernameText.text = username;

			UpdateAfterRoundScore(score);
		}
	}

	public void UpdateAfterRoundScore(int score) {
		Debug.Log("score: " + score);
		afterRoundScoreText.text = "Score: " + score.ToString();
	}

	public void UpdateGameplayScore(int score) {
		Debug.Log("score: " + score);
		gameplayScoreText.text = "Score: " + score.ToString();
	}

	public void ShowNewUserPanel(){
		newUserPanel.SetActive(true);
	}

	public void HideNewUserPanel(){
		newUserPanel.SetActive(false);
	}

	public void SetARWarningPanelVisible(bool visible) {
		if(arWarningPanel.activeSelf != visible) {
			arWarningPanel.SetActive(visible);
		}
	}
}
