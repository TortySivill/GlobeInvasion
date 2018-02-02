using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StartDisplay : MonoBehaviour {
	public  List<GameObject> joinedText;
	private GameManager      gm;
	private int              readyClients;

	public GameObject countdownTextObject;
	private double countdown;
	private bool countdownActive;
	private bool countdownEnd;

	public AudioClip countdownEndClip;

	private AudioSource countdownSound;

	// Use this for initialization
	void Start () {
		foreach (GameObject text in joinedText) {
			Color transparent = text.GetComponent<Text> ().color;
			transparent.a = 0.3f;
			text.GetComponent<Text> ().color = transparent;
			text.GetComponentsInChildren<Text> () [1].enabled = false;
		}

		gm = GameManager.safeFind<GameManager> ();
		readyClients = 0;

		countdown = 6;
		countdownActive = false;
		countdownSound = GetComponent<AudioSource> ();
		countdownEnd = false;
	}

	// Update is called once per frame
	void Update () {
		if (countdownActive) {
			if (countdown <= 0) {
				countdownActive = false;
				countdownSound.Stop ();
				gm.startGame ();
				return;
			} else if (!countdownEnd && countdown <= 1) {
				countdownEnd = true;
				countdownSound.Stop ();
				countdownSound.PlayOneShot (countdownEndClip);
			}

			countdown = countdown - Time.deltaTime;
			setCountDownText();
		}
	}

	public void activateRole(role_struct rs) {
		int joinedTextIndex = (int)rs.team_id - 1;
		if (rs.role == role_id.STRATEGIST)
			joinedTextIndex += 4;

		Color transparent = joinedText [joinedTextIndex].GetComponent<Text> ().color;
		transparent.a = 1.0f;
		joinedText [joinedTextIndex].GetComponent<Text> ().color = transparent;

		abortCountDown ();
	}

	public void deactivateRole(role_struct rs) {
		int joinedTextIndex = (int)rs.team_id - 1;
		if (rs.role == role_id.STRATEGIST)
			joinedTextIndex += 4;

		Color transparent = joinedText [joinedTextIndex].GetComponent<Text> ().color;
		transparent.a = 0.3f;
		joinedText [joinedTextIndex].GetComponent<Text> ().color = transparent;
		if (joinedText [joinedTextIndex].GetComponentsInChildren<Text> () [1].enabled) {
			joinedText [joinedTextIndex].GetComponentsInChildren<Text> () [1].enabled = false;
			readyClients--;
		}
		abortCountDown ();
	}

	public void setCountDownText() {
		Text countdownText = countdownTextObject.GetComponent<Text>();
		countdownText.enabled = true;
		int counter = (int) countdown;
		countdownText.text = counter.ToString();
	}

	public void startCountDown() {
		setCountDownText();
		countdownActive = true;
		GameManager.safeFind<Music> ().chooseTeam.Pause ();
		countdownSound.Play ();
		gm.server.sendCountdownStart();
	}

	public void abortCountDown() {
		countdownActive = false;
		Text countdownText = countdownTextObject.GetComponent<Text>();
		countdownText.enabled = false;
		countdown = 6;
		countdownSound.Stop ();
		countdownEnd = false;
		GameManager.safeFind<Music> ().chooseTeam.UnPause ();
		gm.server.sendCountdownAbort();
	}

	public void roleReady(role_struct rs) {
		int joinedTextIndex = (int)rs.team_id - 1;
		if (rs.role == role_id.STRATEGIST)
			joinedTextIndex += 4;

		Text tick = joinedText [joinedTextIndex].GetComponentsInChildren<Text> ()[1];
		tick.enabled = !tick.enabled;

		readyClients = tick.enabled ? readyClients + 1 : readyClients - 1;
		if (readyClients == gm.connectionMap.Count) {
			Debug.Log ("All Players are ready - Let the fun begin!");
			startCountDown();
		} else {
			if (countdownActive) {
				abortCountDown();
			}
		}
	}
}
