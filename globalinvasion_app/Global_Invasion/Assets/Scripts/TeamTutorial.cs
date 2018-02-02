using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTutorial : MonoBehaviour {

    public Color color;
    public int id;
    public GameObject cursorTutorial;

    private List<CountryTutorial> countries = new List<CountryTutorial>();
    public GameObject Cursor;
    public CountryTutorial stratCurrent;


    // Use this for initialization
    void Start () {
       // GameObject.Find("Cursor").transform.localScale = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update () {
		
	}


    public Color getColor()
    {
        return color;
    }
    
    public List<CountryTutorial> getCountries()
    {
        return countries;
    }

    public void addCountry(CountryTutorial c)
    {
        countries.Add(c);
    }

    public void removeCountry(CountryTutorial c)
    {
        countries.Remove(c);
    }

    public int getTeamById(int id)
    {
        return id;
    }

    public void attackCountry(int troops, CountryTutorial target)
    {
        Debug.Log("attackCountry called");

        int sizeAttacking = troops;
        int sizeDefending = (int)target.troops;

        TeamTutorial teamAttacking = this;
        TeamTutorial teamDefending = target.getOwner();

        //double strengthAttacking = teamAttacking.getTroopStrength();
        //double strengthDefending = teamDefending ? teamDefending.getTroopStrength() : 1.0;

        target.setTroops(sizeAttacking - 8);
        target.setOwner(this);
        Debug.Log(target.getOwner());
        


    }

    public void moveTroops(int troops, CountryTutorial target)
    {
        target.setTroops(troops + target.troops);
    }

    public void instantiateCursor(CountryTutorial origin)
    {
        Debug.Log("got here");
        Vector3 originPosition = origin.transform.position + new Vector3(0, 0.1f, 0);
        Cursor.transform.position =  originPosition;
        Cursor.GetComponent<Renderer>().material.color = color;
        GameObject.Find("Cursor").transform.localScale = new Vector3(10, 10, 10);
        stratCurrent = origin;

        // myCursor = Instantiate(cursorTutorial, originPosition, origin.transform.rotation).GetComponent<CursorTutorial>();
        //myCursor.initialize(originPosition, origin, this);
    }

    public void moveCursor(CountryTutorial target)
    {
        Cursor.transform.position = target.transform.position + new Vector3(0, 0.1f, 0);
        stratCurrent = target;

    }



}
