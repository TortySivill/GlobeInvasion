using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DisplayText : MonoBehaviour {


    private GameManager gm;
    private Team team;
    private Color teamColor;

    public Text TeamScore;
 
    // Use this for initialization
    void Start () {

        gm = GameManager.safeFind<GameManager>();
        team = gm.playerTeam;
        
       

    }
	
	// Update is called once per frame
	void Update ()
    { 

        

        getTeamScore();

    }

   public void getTeamScore()
    {

        string teamName = team.getTeamName();
        teamColor = team.color;
        string scoreText = teamName + ": ";
        string score = team.getTeamScore().ToString();

        TeamScore.text = scoreText + score ;
        teamColor.a = 1.0f;
        TeamScore.GetComponent<Text>().color = teamColor;
        


    }

}
