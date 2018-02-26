using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrialSocketController : MonoBehaviour
{

	public SocketIOComponent socket;
	public List<TrialPlayerController> users;
	private User currentUser;
	public GameObject player;
	private bool allowQuitting = false;

	void Start()
	{
		StartCoroutine(ConnectToServer());
		socket.On(Commands.USER_LOGGED_IN, OnUserLoggedIn);
		socket.On(Commands.USER_DISCONNECTED, OnUserDisconnect);
	}

	IEnumerator ConnectToServer()
	{
		yield return new WaitForSeconds(0.5f);
		socket.Emit(Commands.USER_CONNECT);
	}

	private static JSONObject getJsonObject(string name, string message)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["name"] = name;
		data["message"] = message;
		return new JSONObject(data);
	}

	private static JSONObject getJsonObjectOption(string name, string option)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["option"] = option;
		data["name"] = name;
		return new JSONObject(data);
	}

	public void sendInfo() {
		string name = GameObject.Find ("InputName").GetComponent<InputField> ().text;
		string message = GameObject.Find ("InputMessage").GetComponent<InputField> ().text;
		JSONObject info = getJsonObject(name, message);
		Debug.Log (info.ToString ());
		socket.Emit (Commands.USER_INFO, info);
	}

	public void SendOption() {
		string option = GameObject.Find ("InputMessage").GetComponent<InputField> ().text;
		Debug.Log (currentUser.name);
		JSONObject info = getJsonObjectOption(currentUser.name, option);
		Debug.Log (info.ToString ());
		socket.Emit (Commands.USER_OPTIONS, info);
	}

	private void OnUserLoggedIn(SocketIOEvent obj)
	{
		User user = JsonUtility.FromJson<User>(obj.data.ToString());
		currentUser = new User ();
		currentUser = user;
		player = GameObject.FindGameObjectWithTag ("Player");
		TrialPlayerController playerController = player.GetComponent<TrialPlayerController>();
		playerController = (playerController == null) ? player.AddComponent<TrialPlayerController>() : playerController;
		playerController.externalPlayer = true;
		player.name = user.name;
		playerController.name = user.name;
		this.users.Add(playerController);
	}
		
	private void OnUserDisconnect(SocketIOEvent obj)
	{
		User user = JsonUtility.FromJson<User>(obj.data.ToString());
		Debug.Log (user.name + " disconnected.");	
		TrialPlayerController disconnectedPlayer = users.Find(item => item.name == user.name);
		this.users.Remove (disconnectedPlayer);
		GameObject disconnectedPlayerObject  = disconnectedPlayer.gameObject;
		Destroy(disconnectedPlayer);
		Destroy(disconnectedPlayerObject);
	}

	void OnApplicationQuit()
	{
		StartCoroutine(DelayedQuit());
		if (!allowQuitting)
			Application.CancelQuit();
	}

	IEnumerator DelayedQuit() {
		yield return new WaitForSeconds(2f);
		Debug.Log("Application ending after " + Time.time + " seconds");
		if (currentUser.name != null) {
			JSONObject info = getJsonObjectOption(currentUser.name, currentUser.message);
			socket.Emit (Commands.USER_DISCONNECTED, info);
		}
		allowQuitting = true;
		Application.Quit();
	}
}
