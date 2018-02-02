using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EndDisplay : MonoBehaviour {
	public GameObject Winner;
	public Text Countdown;
	public List<GameObject> TeamScores;

	private float startTime;

	void Start() {
		startTime = Time.fixedTime;
	}

	void Update() {
		int displayTime = (int)(30 - (Time.fixedTime - startTime));

		Countdown.text = displayTime > 0 ?  displayTime.ToString () : "0";
	}

	public void setWinner(Team team) {
		Color c = team.color;
		c.a = 1;
		Text winnerText = Winner.GetComponent<Text> ();
		winnerText.color = c;
		winnerText.text = "TEAM " + team.team_col.ToString() + " WINS";
		Winner.SetActive (true);
	}

	public void setTeamScore(team_id tId, int score, string teamName) {
		string scoreText = teamName + ": "; 
		switch (tId) {
		case team_id.RED:
			TeamScores [0].GetComponent<Text> ().text = scoreText + score.ToString();
			TeamScores [0].SetActive (true);
			break;
		
		case team_id.BLUE:
			TeamScores [1].GetComponent<Text> ().text = scoreText + score.ToString();
			TeamScores [1].SetActive (true);
			break;

		case team_id.GREEN:
			TeamScores [2].GetComponent<Text> ().text = scoreText + score.ToString();
			TeamScores [2].SetActive (true);
			break;

		case team_id.YELLOW:
			TeamScores [3].GetComponent<Text> ().text =  scoreText + score.ToString();
			TeamScores [3].SetActive (true);
			break;
		}
	}
}
