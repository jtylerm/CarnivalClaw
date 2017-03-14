using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public static UI_Manager defaultUI_Manager;

	private GameObject afterRoundInfoPanel;


	void Awake() {
		defaultUI_Manager = this;
		afterRoundInfoPanel = GameObject.Find("Canvas").transform.Find("AfterRoundInfoPanel").gameObject;
	}

	void Start () {
		
	}

	void Update () {
		
	}

	public void UpdateForGameState(GameState gameState, string username, int score) {
		if(gameState == GameState.IN_ROUND) {
			afterRoundInfoPanel.gameObject.SetActive(false);
		}
		else {
			afterRoundInfoPanel.gameObject.SetActive(true);

			Text usernameText = GameObject.Find("Canvas").transform.Find("AfterRoundInfoPanel").transform.Find("UsernameText").GetComponent<Text>();
			usernameText.text = username;

			Text scoreText = GameObject.Find("Canvas").transform.Find("AfterRoundInfoPanel").transform.Find("ScoreText").GetComponent<Text>();
			scoreText.text = "Score: " + score.ToString();
		}

	}
}
