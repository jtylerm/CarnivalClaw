using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerUpdateResponse : System.Object {

	public GameState gameState;
	public GameState nextGameState;
	public int timeRemaining;
	public string currentRoundID;
}
