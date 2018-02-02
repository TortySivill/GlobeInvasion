using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using System.Text;

public class Finish : MonoBehaviour {
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

	public Text endText;
	private bool outcome;

	public void Initialize(bool won){
		outcome = won;
		SetText(outcome);
	}
		
	void SetText (bool outcome)
	{
		if (outcome == true) 
		{
			endText.text = "You Win!";
		}
		if(outcome == false)
		{
			endText.text = "You lose";
		}
	}

}
	