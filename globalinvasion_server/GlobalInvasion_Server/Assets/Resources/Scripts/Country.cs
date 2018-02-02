/* Country.cs */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Country : MonoBehaviour {
    public  int           id;
    public  List<Country> neighbours;
	public  List<Country> stratNeighbours;
	public GameObject     CountryText;

    public bool port;
    public GameObject Bridge_a;
    public GameObject Bridge_b;
    public GameObject Bridge_c;

    private Renderer      rend;
	private GameObject    troopTile;
    private TextMesh      textMesh;
	private GameObject    bar;
    private Team          owner;

    private double        rateMoney;
    private double        rateTroops;
    private double        troops;

	private const int LEVEL9 = 999;
	private const int LEVEL8 = 599;
	private const int LEVEL7 = 349;
	private const int LEVEL6 = 199;
	private const int LEVEL5 = 99;
	private const int LEVEL4 = 49;
	private const int LEVEL3 = 19;
	private const int LEVEL2 = 9;

    void Awake () {
		troopTile = Instantiate (CountryText, this.transform.position, CountryText.transform.rotation);
		textMesh = troopTile.GetComponentInChildren<TextMesh>();
		bar = troopTile.transform.GetChild (0).gameObject.transform.GetChild (0).gameObject;
   
        // Initialise random money and troop rate at game start
        float random        = UnityEngine.Random.value * 0.5f;
        rateMoney           = (random + 0.25) * neighbours.Count * 2;
        rateTroops          = (0.75 - random) * neighbours.Count * 0.2;
        troops              = neighbours.Count * 3;
		rend = GetComponent<Renderer>();

		displayTroops ();
        //hide all bridges on start

       if (this.port)
        {
            Bridge_a.SetActive(false);
            Bridge_b.SetActive(false);
            Bridge_c.SetActive(false);
        }

        // Country starts with no owner
        owner               = null;
    }

    // If a country is owned, give the team money and troops
    void Update() {
    }

    // If a country is owned, give the team money and troops
    public void increaseResources() {
        if (owner) {
            troops = troops + rateTroops;
            owner.increaseMoney(rateMoney);

			displayTroops ();
        }
    }

    public void addTroops(double dt) {
        troops = troops + dt;
		displayTroops ();
    }

    public void removeTroops(double dt) {
        troops = troops - dt;
		displayTroops ();
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

	public List<Country> getStratNeighbours() {
		return stratNeighbours;
	}

    public double getTroops() {
        return troops;
    }

	public void setTroops(double t) {
		troops = t;
		displayTroops ();
	}

    public void setOwner(Team team) {
        Debug.Log(this.name + " owned by " + team.name);
		if (owner)
			owner.removeCountry (this);
        owner               = team;

		Color32 newColor = owner.getColor();
		newColor.a = 150;
		rend.material.color = newColor;

		team.addCountry(this);
    }
    public int checkBridgeLevel()
    {
        if (Bridge_a.activeSelf) {
            return 1;
        } else if (Bridge_b.activeSelf) {
            return 2;
        } else if (Bridge_c.activeSelf) {
            return 3;
        } else {
            return 0;
        }
    }
    
    public bool checkPort()
    {
        return port;
    }


    public void showBridgeA(Color team_color)
    {
        Bridge_a.GetComponent<Renderer>().materials[0].color = team_color;
        Bridge_a.SetActive(true);
    }

    public void showBridgeB(Color team_color)
    {
        Bridge_b.GetComponent<Renderer>().materials[2].color = team_color;
        Bridge_b.SetActive(true);
    }

    public void showBridgeC(Color team_color)
    {
		Bridge_c.GetComponent<Renderer>().materials[4].color = team_color;
        Bridge_c.SetActive(true);
    }

    public void hideBridgeA()
    {
        Bridge_a.SetActive(false);
    }


    public void hideBridgeB()
    {
        Bridge_b.SetActive(false);
    }


    public void hideBridgeC()
    {
        Bridge_c.SetActive(false);
    }

    public void displayTroops() {
		int level = 0;
		float barSize = 0;

		if (troops > LEVEL9) {
			level = 9;
			barSize = 1.0f;
		} 
		else if (troops > LEVEL8) {
			level = 8;
			barSize = (float)((troops - LEVEL8) / (LEVEL9 - LEVEL8));
		}
		else if (troops > LEVEL7) {
			level = 7;
			barSize = (float)((troops - LEVEL7) / (LEVEL8 - LEVEL7));
		}
		else if (troops > LEVEL6) {
			level = 6;
			barSize = (float)((troops - LEVEL6) / (LEVEL7 - LEVEL6));
		}
		else if (troops > LEVEL5) {
			level = 5;
			barSize = (float)((troops - LEVEL5) / (LEVEL6 - LEVEL5));
		}
		else if (troops > LEVEL4) {
			level = 4;
			barSize = (float)((troops - LEVEL4) / (LEVEL5 - LEVEL4));
		}
		else if (troops > LEVEL3) {
			level = 3;
			barSize = (float)((troops - LEVEL3) / (LEVEL4 - LEVEL3));
		}
		else if (troops > LEVEL2) {
			level = 2;
			barSize = (float)((troops - LEVEL2) / (LEVEL3 - LEVEL2));
		}
		else if (troops > 0) {
			level = 1;
			barSize = (float)(troops / LEVEL2);
		}
		else {
			level = 0;
			barSize = 0;
		}

		textMesh.text       = level.ToString();
		Vector3 localScale = bar.transform.localScale;
		localScale.x = barSize;
		bar.transform.localScale = localScale;
	}
}
