using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Text;

public class Finish {
	private List<playerScore> leaderboard = new List<playerScore>();
	private List<playerScore> unsortedLeaderboard = new List<playerScore>();

	public Finish() {
	}

	[Serializable]
	public struct playerScore
	{
		public string name;
		public int score;

		public playerScore(string p1, int p2)
		{
			name = p1;
			score = p2;
		}
	}

	public void writeScore(int score, string name)
	{
		using (StreamWriter w = File.AppendText ("scores.txt")) {
			w.WriteLine (name + ";" + score.ToString ());
			w.Close();
		}
	}

	void readScore()
	{
		unsortedLeaderboard.Clear ();
		string[] lines = System.IO.File.ReadAllLines("scores.txt");
		foreach (string line in lines)
		{
			string[] words = line.Split(';');
			playerScore temp = new playerScore (words [0], Int32.Parse(words[1]));
			unsortedLeaderboard.Add (temp);
		}
		leaderboard = unsortedLeaderboard.OrderByDescending(o=>o.score).ToList();
	}

	public List<playerScore> getTopTen() {
		readScore ();
		if (leaderboard.Count <= 10)
			return leaderboard;
		List<playerScore> topTen = leaderboard.GetRange (0, 10);
		return topTen;
	}
}
