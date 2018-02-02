using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTutorial : MonoBehaviour
{
    private Vector3 currentPosition;
    public CountryTutorial current;
    private TeamTutorial owner;
    private StrategistTutorial sm;

    private Color team_color;

    public CursorTutorial()
    {

    }

    // Use this for initialization
    void Start()
    {
        sm = StrategistTutorial.FindObjectOfType<StrategistTutorial>();

    }

    public void initialize(Vector3 currentPosition, CountryTutorial current, TeamTutorial owner)
    {
        this.current = current;
        this.owner = owner;
        team_color = owner.color;

        GetComponent<Renderer>().material.color = team_color;

        currentPosition = this.transform.localPosition;
    }

    public void move(CountryTutorial target)
    {
        if (target == current)
        {
            Debug.Log("Invalid Move. Try Again.");
        }
        else
        {
            this.transform.localPosition = target.transform.position + new Vector3(0, 2, 0);
            current = target;
            Debug.Log("Moved to " + target);
        }
    }

    public void select()
    {

    }

    public CountryTutorial getCurrent()
    {
        return current;
    }

    public void setCurrent(CountryTutorial newCurrent)
    {
        current = newCurrent;
    }

    public TeamTutorial getOwner()
    {
        return owner;
    }

}
