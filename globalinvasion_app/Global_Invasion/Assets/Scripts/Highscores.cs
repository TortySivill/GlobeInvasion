using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using System.Text;


public class Highscores : MonoBehaviour {

	public List<Finish.playerScore> leaderboard;
	public GameObject ScoreEntry;
	public GameObject ScrollContain;

	public void Initialize(List<Finish.playerScore> topTen){
		leaderboard = topTen;
		fillLeaderboard (leaderboard);
	}
		
	void fillLeaderboard(List<Finish.playerScore> leaderboard )
	{
		//while (GameObject.Find("ScoreEntry") != null) Destroy(GameObject.Find("ScoreEntry"));

		for (int i = 0; i< leaderboard.Count; i++) {
			GameObject scoreEntry = Instantiate (ScoreEntry) as GameObject;
			scoreEntry.transform.SetParent(ScrollContain.transform);
			scoreEntry.transform.localScale = ScrollContain.transform.localScale;
			setEntry (scoreEntry, i + 1, leaderboard [i].name, leaderboard [i].score); 
		}
	}

	void setEntry (GameObject scoreEntry, int position, string name, int score)
	{
		Text[] entry = scoreEntry.GetComponentsInChildren<Text> ();
		entry[0].text = name;
		entry[1].text = score.ToString ();
		entry[2].text = position.ToString ();

	}
}
