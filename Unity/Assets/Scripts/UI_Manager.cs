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

	public GameObject instructionsPanel;

	public Text usernameText;
	public Text gameplayScoreText;


	void Awake() {
		defaultUI_Manager = this;

//		afterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("AfterRoundInfoPanel").gameObject;
//		normalAfterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("NormalAfterRoundInfoPanel").gameObject;
//		normalAfterRoundInfoPanel.SetActive(true);
//		recordBreakAfterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("RecordBreakAfterRoundInfoPanel").gameObject;
//		normalUsernameText = afterRoundInfoPanel.transform.Find("NormalUsernameText").GetComponent<Text>();
//		normalAfterRoundScoreText = afterRoundInfoPanel.transform.Find("NormalScoreText").GetComponent<Text>();
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

		//tell user to aim camera at orientation image
		this.ShowInstructionsPanel();
	}

	public void ShowInstructionsPanel(){
		instructionsPanel.SetActive(true);
	}

	public void HideInstructionsPanel(){
		instructionsPanel.SetActive(false);
		GameManager.defaultGameManager.DidHideInstructionsPanel();
	}

	public void ShowGameplayPanel(){
		gameplayUIPanel.SetActive(true);
	}

	public void HideGameplayPanel(){
		gameplayUIPanel.SetActive(false);
	}

	public void SetUsername(string username){
		usernameText.text = username;
	}

	public void SetAfterRoundInfoVisible(bool visible) {
		if(afterRoundInfoPanel.activeSelf != visible) {
			afterRoundInfoPanel.SetActive(visible);
		}
	}

	public void SetARWarningPanelVisible(bool visible) {
		if(arWarningPanel.activeSelf != visible) {
			arWarningPanel.SetActive(visible);
		}
	}
}
