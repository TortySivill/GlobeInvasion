/* Team.cs */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Team : MonoBehaviour {
	
	public  Color 			color;

	private GameManager 	gm;
    private double        	money;
    private List<Country> 	countries         = new List<Country>();
    private List<Country> 	selectedCountries = new List<Country>();

    private double        	troopStrength;
    private double        	sendTroopRatio;

	private Country         feedbackCountry;
    private string          teamName ;
    private int             teamScore;

	void Start () {
        gm = GameManager.safeFind<GameManager>();
        teamName = " ";
        teamScore = 0;
    }

    public void addCountry(Country c) {
        countries.Add(c);
		Debug.Log ("Country added");
    }

	public void removeCountry(Country c) {
		countries.Remove(c);
	}

    public Color getColor() {
        return color;
    }

    public List<Country> getCountries() {
        return countries;
    }

    public int[] getSelectedCountryIds() {
        int[] selectedCountryIds = new int[selectedCountries.Count];
        
        for (int cc = 0; cc < selectedCountries.Count; cc++) {
            selectedCountryIds[cc] = selectedCountries[cc].getId();
        }

        return selectedCountryIds;
    }

    public double getMoney() {
        return money;
    }

	public void updateMoney(int money) {
		if (money <= 0) {
			Handheld.Vibrate ();
		} else {
			this.money = money;
		}
	}

    public void updateTeamDetails(int teamScore, string teamName)
    {
        this.teamScore = teamScore;
        this.teamName = teamName;
    }

	private IEnumerator showFeedbackInvalid(Country target) {
		Debug.Log("Coroutine started: invalid");
		target.GetComponent<Renderer>().material.color = Color.magenta;
		yield return new WaitForSeconds(0.05f);
		target.resetColor();
		yield return new WaitForSeconds(0.05f);
		target.GetComponent<Renderer>().material.color = Color.magenta;
		yield return new WaitForSeconds(0.05f);
		target.resetColor();
	}

	private IEnumerator showFeedbackValid(Country target) {
		Debug.Log("Coroutine started: valid");
		target.GetComponent<Renderer>().material.color = Color.white;
		feedbackCountry = target;
		yield return new WaitForSeconds(0.1f);
		target.resetColor();
	}

    public void feedback(packet_feedback pf)
    {
        if (pf.valid)
        {
            StartCoroutine(showFeedbackValid(gm.getCountryByID(pf.countryId)));
        } else
        {
            Handheld.Vibrate();
            StartCoroutine(showFeedbackInvalid(gm.getCountryByID(pf.countryId)));
        }
    }

    public void moveArea(Country target) {
        if (selectedCountries.Count == 0)
        {
            Debug.Log("You have to select a country");
			Handheld.Vibrate ();
			StartCoroutine(showFeedbackInvalid(target));

            return;
        }
        List<Country> targetArea = target.neighbourhoodArea(this);
		Debug.Log ("Countries in area on move: " + targetArea.Count + " countries in selectedCountries: " + selectedCountries.Count);
        int[] selectedCountryIds = getSelectedCountryIds();
		gm.client.moveTroops(selectedCountryIds, target.getId(), team_id.DEFAULT);

        clearSelectedCountries();
    }

    public void selectCountry(Country c) {
        if (selectedCountries.Contains(c)) {
			removeFromSelectedCountries(c);
			c.unsetSelected();
        } else {
			addToSelectedCountries(c);
			Debug.Log("Selected country: " + c);
        }
    }

	public void selectCountryArea(Country c) {
		List<Country> neighbourArea = new List<Country>();
		neighbourArea.Add (c);
		foreach (Country country in c.neighbours) {
			if (country.getOwner () == this)
				neighbourArea.Add (country);
		}

        foreach(Country country in neighbourArea)
        {
			if(!selectedCountries.Contains(country))
            	addToSelectedCountries(country);
        }
	}

	private void addToSelectedCountries(Country c) {
		selectedCountries.Add(c);
		c.setSelected();
	}

	private void removeFromSelectedCountries(Country c) {
		selectedCountries.Remove(c);
		c.unsetSelected();
	}

	public void checkSelectedAndRemove(Country c) {
		if(selectedCountries.Contains(c)) {
			removeFromSelectedCountries(c);
		}
	}

	public void clearSelectedCountries() {
		foreach(Country c in selectedCountries) {
			c.unsetSelected ();
		}
		selectedCountries.Clear();
	}

	// Cursor Control
	public void MoveHorizontal(float hInput) 
	{
		if (hInput == 1)
			gm.client.moveCursor (1);
		else
			gm.client.moveCursor (3);
	}

	public void MoveVertical(float vInput) 
	{
		if (vInput == 1)
			gm.client.moveCursor (0);
		else
			gm.client.moveCursor (2);
	}

	public void dropBomb() 
	{
		gm.client.moveCursor (5);
	}

	public void increaseTroops()
	{
		gm.client.moveCursor (6);
	}

    public void buildBridge()
    {
        gm.client.buildBridge(true);
       
    }

   
    public int getTeamScore()
    {
        return teamScore;
    }

    public String getTeamName()
    {
        return teamName;
    }
  



}
