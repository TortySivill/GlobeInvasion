using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DisplayText : MonoBehaviour {

    private GameManager gm;
    public Text TimeRemaining;
    private float timeLeft;
    int minutes = 0;
    int seconds = 0;

    public List<Text> TeamList;
 
    // Use this for initialization
    void Start () {

        gm = GameManager.safeFind<GameManager>();
        timeLeft = gm.getGameTime() *60f+1;

    }
	
	// Update is called once per frame
	void Update () {

        if (timeLeft > 0f)
        {
            timeLeft = timeLeft - Time.deltaTime;
            minutes = Mathf.FloorToInt(timeLeft / 60f);
            seconds = Mathf.FloorToInt(timeLeft % 60f);

            TimeRemaining.text = minutes + ":" + seconds.ToString("00");
        }

        else
        {
            TimeRemaining.text = "0:00";
        }

        getTeamScore();

    }

   public void getTeamScore()
    {
            List<string> scoreTexts = new List<string>();
            List<string> scores = new List<string>();
            List<Color> colours = new List<Color>();

		foreach (Team t in FindObjectsOfType<Team>())
        {
			if (!gm.getActiveTeams ().Contains (t.team_col) && !gm.getFinishedTeams ().Contains (t.team_col))
				continue;

            string teamName = t.getName();
            Color teamColor = t.color;
            string scoreText = teamName + ": ";
			string score = gm.calculateScore(t).ToString();

            colours.Add(teamColor);
            scoreTexts.Add(scoreText);
            scores.Add(score);

        }

        int i = 0;

        foreach (team_id tId in gm.getActiveTeams())
        {
            TeamList[i].text = scoreTexts[i] + scores[i];
            Color teamColor = colours[i];
            teamColor.a = 1.0f;
            TeamList[i].GetComponent<Text>().color = teamColor;
            
            i++;
        }
    }

}
