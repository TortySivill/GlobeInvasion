/* Country.cs */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Country : MonoBehaviour {
    public  int           id;
    public  List<Country> neighbours;

    private Renderer      rend;
    private Team          owner;

    private Team          playerTeam;
	private List<Country> teamArea;

    void Awake() {
        rend                = GetComponent<Renderer>();
        playerTeam          = GameManager.safeFind<Team>();
   
        // Country starts with no owner
		if(owner != playerTeam)	
        	owner           = null;
		rend.material.color = new Color (0.6f, 0.6f, 0.6f, 1);
    }

    // If a country is owned, give the team money and troops
    void Update() {

    }

    public int getId() {
        return id;
    }

    public Team getOwner() {
        return owner;
    }

    public List<Country> getNeighbours() {
        return neighbours;
    }

    public void resetColor() {
        if (owner) {
            rend.material.color = owner.getColor();        
        } else {
			rend.material.color = new Color (0.6f, 0.6f, 0.6f, 1);
        }
    }

    public void setOwner(Team team) {
		if (owner == team)
			return;
		owner = team;
		if (owner == playerTeam) {
			rend.material.color = owner.getColor ();
			team.addCountry(this);
			//int countCountries = team.getCountries ().Count;
			//Debug.Log("Countries = " + countCountries);
		} else {
			rend.material.color = new Color (0.6f, 0.6f, 0.6f, 1);
			team.removeCountry(this);
		}
    }

    public void onClick() {
		Debug.Log(this.name + " clicked");

		if (owner == playerTeam) {
			owner.selectCountry(this);
		} else {
			playerTeam.moveArea(this);
		}		
    }

    public void onDoubleClick() {
    	Debug.Log(this.name + " double clicked");

    	if (owner == playerTeam)
    		owner.selectCountryArea(this);
    }

    public void onLongClick() {
    	Debug.Log(this.name + " long clicked");
        playerTeam.moveArea(this);
    }

	public void setSelected() {
		Color color         = owner.getColor();
		rend.material.color = new Color (color.r - 0.5f, color.g - 0.5f, color.b - 0.5f);
	}

	public void unsetSelected() {
		rend.material.color = owner.getColor ();
	}

	public List<Country> neighbourhoodArea(Team areaTeam) {
		teamArea = new List<Country>();
        addCountryToArea(this, areaTeam);
		return teamArea;
    }

	void addCountryToArea(Country seed, Team areaTeam)
    {
        teamArea.Add(seed);
        foreach (Country c in seed.neighbours)
        {
            if (c.getOwner() == areaTeam && !teamArea.Contains(c))
            {
                addCountryToArea(c, areaTeam);
            }
        }
    }

}
