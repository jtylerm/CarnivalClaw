using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

public class WebRequestManager : System.Object {

	//public static string BASE_URL = "http://192.168.1.67:8081";
	public static string BASE_URL = "http://groupargame-dev.us-east-1.elasticbeanstalk.com";

	public static string WEB_API_PATH = BASE_URL + "/api";

	public static string WEB_ENDPOINT_NEW_GUEST_USER = WEB_API_PATH + "/guestuser/new";
	public static string WEB_ENDPOINT_PLAYER_UPDATE = WEB_API_PATH + "/player/update";

	private int playerUpdateID = 1;

	public IEnumerator GetNewUser() {
		//yield return new WaitForEndOfFrame();

		string url = WEB_ENDPOINT_NEW_GUEST_USER;
		WWWForm form = new WWWForm();
		WWW web = new WWW(url);
		yield return web;

		if(!string.IsNullOrEmpty(web.error)) {
			Debug.Log("GetNewUser: " + web.error);

			Value props = new Value();
			props["error"] = new Value(web.error);
			Mixpanel.Track("WebRequestManager.GetNewUser.Failed", props);
		}
		else {
			Debug.Log("guest user json: " + web.text);

			GuestUserResponse response = JsonUtility.FromJson<GuestUserResponse>(web.text);

			int id = response.id;
			string username = response.username;

			GameManager.defaultGameManager.DidGetNewUser(id, username);
		}
	}

	public IEnumerator SendPlayerUpdate(int orientationImageIndex, int playerID, int score, string roundID) {
		//yield return new WaitForEndOfFrame();

		playerUpdateID++;

		string url = WEB_ENDPOINT_PLAYER_UPDATE + "?update_id=" + playerUpdateID 
			+ "&orientation_image_index=" + orientationImageIndex 
			+ "&player_id=" + playerID
			+ "&score=" + score
			+ "&round_id=" + roundID;
		//Debug.Log("SendPlayerUpdate - URL: " + url);

		WWWForm form = new WWWForm();
		WWW web = new WWW(url);
		yield return web;

		if(!string.IsNullOrEmpty(web.error)) {
			Debug.Log("SendPlayerUpdate: " + web.error);

			Value props = new Value();
			props["error"] = new Value(web.error);
			Mixpanel.Track("WebRequestManager.SendPlayerUpdate.Failed", props);
		}
		else {
			Debug.Log("player update json: " + web.text);

			PlayerUpdateResponse response = JsonUtility.FromJson<PlayerUpdateResponse>(web.text);
		
			GameManager.defaultGameManager.DidGetGameUpdate(response.gameState, response.nextGameState, response.timeRemaining, response.currentRoundID);
		}
	}
}