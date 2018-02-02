/* Team.cs */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour {
	public  GameManager   gm;
	public  Color         color;
	public  GameObject    cursor;
   

	private ShortestPath  pathManager;
	private double        money;
	private List<Country> countries = new List<Country>();

	private double        troopStrength;
	private double        sendTroopRatio;
	private Cursor        myCursor;

	public team_id       team_col;
	public string        commanderName;
	public string        strategistName;

    private int         teamScore;

	void Start () {
		money         = 1000;
		troopStrength = 1;
		gm            = GameManager.safeFind<GameManager>();
		pathManager   = GameManager.safeFind<ShortestPath>();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	// Add country to list of countries
	public void addCountry(Country c) {
		countries.Add(c);
	}

	public void removeCountry(Country c) {
		countries.Remove (c);
		if (countries.Count == 0)
			gm.checkTeamOut(this);
	}

	public Color getColor() {
		return color;
	}

	public List<Country> getCountries() {
		return countries;
	}

	public int getCountryCount() {
		return countries.Count;
	}

	public double getTroopStrength() {
		return troopStrength;
	}

	public void increaseMoney(double n) {
		money = money + n;
	}

	public int getMoneyAsInt() {
		return (int)money;
	}

	// Attack country targetId with countries attackingCountryIds
	public void attackCountry(int troops, Country target) {
		Debug.Log("attackCountry called");

		int sizeAttacking = troops;
		int sizeDefending = (int) target.getTroops();

		Team   teamAttacking     = this;
		Team   teamDefending     = target.getOwner();

		double strengthAttacking = teamAttacking.getTroopStrength();
		double strengthDefending = teamDefending ? teamDefending.getTroopStrength() : 1.0;

		// Simulate battle
		if (chooseWinner(strengthAttacking, ref sizeAttacking, strengthDefending, ref sizeDefending)) {
            // Attacking team has won
            target.setTroops(sizeAttacking);
			gm.setCountryOwner(teamAttacking.team_col, target, sizeAttacking);

			gm.checkStrategistOnCountry (target, teamAttacking);

            gm.server.conquerCountry(target.getId(), teamAttacking.team_col, gm.getCommander(teamAttacking), sizeAttacking);
			if (teamDefending) {
				gm.server.conquerCountry (target.getId (), teamAttacking.team_col, gm.getCommander (teamDefending), sizeAttacking);
				gm.checkStrategistOnCountry (target, teamDefending);
			}
		} else {
			// Defending team has won
			target.setTroops(sizeDefending);
			if(teamDefending)
				gm.setCountryOwner(teamDefending.team_col, target, sizeDefending);
			else
				gm.setCountryOwner(team_id.DEFAULT, target, sizeDefending);
		}
	}

	// Move troops from countries moveFromIds to country moveToId
	public void moveTroops(int[] moveFromIds, int moveToId) {
		Country target = gm.getCountryById (moveToId);
		foreach (int ii in moveFromIds) {
			if (ii == moveToId)
				continue;
			Country c = gm.getCountryById(ii);
			double troops = c.getTroops();	
			if (troops < 1)
				continue;
			
			pathManager.addJob(c, target, (int)troops, this);
		}
	}

	// Simulate a battle and return true if the attacking team wins. sizeAttacking/sizeDefending holds remaining troop size.
	bool chooseWinner(double strengthAttacking, ref int sizeAttacking, double strengthDefending, ref int sizeDefending) {
		while (sizeAttacking > 0 && sizeDefending > 0) {
			if (UnityEngine.Random.value * (strengthAttacking + strengthDefending) < strengthAttacking) {
				sizeDefending--;
			} else {
				sizeAttacking--;
			}
		}

		return sizeAttacking > 0;
	}


	public void instantiateCursor(Country origin) {
		Vector3 originPosition = origin.transform.position + new Vector3 (0, 0.1f, 0);
		myCursor = Instantiate(cursor, originPosition, Quaternion.identity).GetComponent<Cursor>();
		myCursor.initialize(originPosition, origin, this);
	}    

	public void moveCursor (int target) {
		Country current = myCursor.getCurrent ();
		List<Country> neighbours = current.getStratNeighbours ();        
		if (target == 5) { // Drop bomb
			if (!countries.Contains(current) && money >= 250) {
				money = money - 250;

				Debug.Log ("send plane");
				gm.instantiatePlane(current, this);

				gm.server.sendPlaneToProjectors(current, this);
			Debug.Log (this + " dropped a bomb on " + current);            
			} else {
				Debug.Log ("You can't drop bombs on your own country");
				gm.server.sendInvalidStratMovement(gm.getStrategist (this));
			} 
		} else if (target == 6) {
			if (countries.Contains (current) && money >= 250) {
				money = money - 250;
				current.addTroops (25);
				gm.instantiateSparkles (current);
				Debug.Log (this + " increased it's troops!");            
			} else {
				Debug.Log ("You can't give troops to other teams");
				gm.server.sendInvalidStratMovement(gm.getStrategist (this));
			}
		} else {
			//Debug.Log ("Bridge check: " + gm.bridgeBuildingPossible (this, neighbours [target]));
			gm.server.sendBridgeCheck(gm.getStrategist(this), gm.bridgeBuildingPossible(this, neighbours[target]));

			myCursor.move(neighbours [target]);
			gm.server.sendCursorToProjectors(this, neighbours[target]);
		}
	}

	public void buildBridge()
    {
        Debug.Log("trying to build bridge");
        Country current = myCursor.getCurrent();
		if (!current.port)
			return;
        int level = current.checkBridgeLevel();
		bool connectedToBridge = false;
		if (countries.Contains (current.Bridge_a.GetComponent<Bridge> ().port1)
		   || countries.Contains (current.Bridge_a.GetComponent<Bridge> ().port2)) {
			connectedToBridge = true;
		}

		if (connectedToBridge && current.port && level < 3)
        {
			if (level == 0)
            {
                current.showBridgeA(getColor());
				gm.server.sendBridgeToProjectors(team_col, current.id, 1);

				current.Bridge_a.GetComponent<Bridge> ().setInvulnerable ();

				current.Bridge_a.GetComponent<Bridge>().owner = this;
				current.Bridge_b.GetComponent<Bridge>().owner = this;
				current.Bridge_c.GetComponent<Bridge>().owner = this;
            }
			else if (level == 1 && current.Bridge_a.GetComponent<Bridge>().owner == this)
            {
                current.hideBridgeA();
                current.showBridgeB(getColor());
				gm.server.sendBridgeToProjectors(team_col, current.id, 2);

				current.Bridge_b.GetComponent<Bridge> ().setInvulnerable ();

				current.Bridge_a.GetComponent<Bridge>().owner = this;
				current.Bridge_b.GetComponent<Bridge>().owner = this;
				current.Bridge_c.GetComponent<Bridge>().owner = this;
            }
			else if(level == 2 && current.Bridge_a.GetComponent<Bridge>().owner == this)
            {
                current.hideBridgeB();
                current.showBridgeC(getColor());
				gm.server.sendBridgeToProjectors(team_col, current.id, 3);

				gm.playBridgeFininshed ();
				gm.server.sendBridgeCheck (gm.getStrategist (this), false);
				current.Bridge_c.GetComponent<Bridge> ().setInvulnerable ();

				current.Bridge_a.GetComponent<Bridge>().owner = this;
				current.Bridge_b.GetComponent<Bridge>().owner = this;
				current.Bridge_c.GetComponent<Bridge>().owner = this;
            }
			else
			{
				Debug.Log("Bridge building failed");
				gm.server.sendInvalidStratMovement(gm.getStrategist(this));

			}
        }

        else
        {
            Debug.Log("Bridge building failed");
            gm.server.sendInvalidStratMovement(gm.getStrategist(this));

        }
    }

    public void checkPath(int troops, Country target, Team owner, Country source, List<Country> pathList)
    {
        int check = 0;
  
        for (int i = 1; i < pathList.Count; i++)
        {
            if (pathList[i].checkPort() && pathList[i-1].checkPort())
            {
				if ((pathList [i].id == 27 && pathList [i - 1].id == 29) || (pathList [i].id == 29 && pathList [i - 1].id == 27))
					continue;
				
                if (pathList[i].checkBridgeLevel() < 3)
                {
                    check = 1;
                }
                else if (pathList[i].Bridge_c.GetComponent<Renderer>().materials[4].color != getColor())
                {
                    check = 1;
                }

            }
        }
        if (check == 1)
        {
            Debug.Log("invlaid path");
            gm.server.sendCommanderFeedback(gm.getCommander(this), false, target.id);
        }
        else
        {
            source.removeTroops(troops);
            gm.instantiateTank(troops, target, owner, source, pathList);
            gm.server.sendCommanderFeedback(gm.getCommander(this), true, target.id);
        }


        //gm.instantiateTank(troops, target, owner, source, pathList);
    }


    public int getTroopCount() {
		double troops = 0;
		foreach (Country c in countries) {
			troops += c.getTroops ();
		}
		troops += gm.troopsInTanks (this);
		return (int)troops;
	}

	public void setCommanderName(string s) {
		commanderName = s;
	}

	public void setStrategistName(string s) {
		strategistName = s;
	}

	public string getName() {
		return commanderName +  " & " + strategistName;
	}

	public string getCommanderName() {
		return commanderName;
	}

	public string getStrategistName() {
		return strategistName;
	}

    public int getTeamScore()
    {
        return gm.calculateScore(gm.getTeamById(team_col));
    }

	public Country getCursorCountry() {
		return myCursor.getCurrent ();
	}
}
