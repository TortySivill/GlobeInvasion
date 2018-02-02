using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryTutorial : MonoBehaviour
{
    public int id;
    public int troops;
    public GameObject CountryText;
    public List<CountryTutorial> neighbours;
    public List<CountryTutorial> stratNeighbours;

    public CommanderTutorial cm;
    private GameObject troopTile;
    private TextMesh textMesh;
    private GameObject bar;
    private TeamTutorial owner;

    private float timer = 0f;
   


    private float lastClick = 0;
    private float waitTime = 1.0f; //wait time befor reacting
    private float downTime; //internal time from when the key is pressed
    private bool isHandled = false;

    private const int LEVEL9 = 999;
    private const int LEVEL8 = 599;
    private const int LEVEL7 = 349;
    private const int LEVEL6 = 199;
    private const int LEVEL5 = 99;
    private const int LEVEL4 = 49;
    private const int LEVEL3 = 19;
    private const int LEVEL2 = 9;


    // Use this for initialization
    void Start()
    {
        owner = null;
        cm = CommanderTutorial.FindObjectOfType<CommanderTutorial>();
        if (this.id == 1)
        {
            troops = 30;
        }
        else if (this.id == 12)
        {
            troops = 40;
        }
        else
        {
            troops = Random.Range(5, 20);
        }
        troopTile = Instantiate(CountryText, this.transform.position, this.transform.rotation);
        textMesh = troopTile.GetComponentInChildren<TextMesh>();
        bar = troopTile.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        displayTroops();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton (0)){
            timer += 1;
        }
        

    }

    public int getId()
    {
        return id;
    }

    public List<CountryTutorial> getNeighbours()
    {
        return neighbours;
    }


    public void setOwner(TeamTutorial team)
    {
        this.GetComponent<MeshRenderer>().materials[0].color = team.getColor();
        team.addCountry(this);
        owner = team;
    }

    public TeamTutorial getOwner()
    {
        return owner;
    }

    public void OnMouseDown()
    {
       
        timer = Time.time;
       
  
      

    }


    public void OnMouseUp()
    {
        float timeup = Time.time;
        manageClicks(timeup);
    }

    public void manageClicks(float timeup)
    {
        //isHandled = false;
        if(this.id == 0) {
            if (timeup - timer < -20.0)
            {

                Debug.Log("this is working");
                Debug.Log(timeup - timer);
                cm.manageLongClick(this);
            }
            else
            {
                cm.manageClickEvent(this);
            }

        }
        else
        {

            //look for a double click
            if (Time.time - lastClick < 0.3)
            {
                // do something
                cm.manageDoubleClickEvent(this);
            }
            else
            {
                Debug.Log("click event");
                cm.manageClickEvent(this);
            }
        }
        lastClick = Time.time;
        timer = 0;
    }

    public void setTroops(int t)
    {
        troops = t;
        displayTroops();
    }


    IEnumerator Flasher()
    {
        for (int i = 0; i < 5; i++)
        {
            this.GetComponent<MeshRenderer>().materials[0].color = Color.black;
            yield return new WaitForSeconds(.1f);
            this.GetComponent<MeshRenderer>().materials[0].color = getOwner().color;
            yield return new WaitForSeconds(.1f);
        }
    }

    public void displayTroops()
    {
        int level = 0;
        float barSize = 0;

        if (troops > LEVEL9)
        {
            level = 9;
            barSize = 1.0f;
        }
        else if (troops > LEVEL8)
        {
            level = 8;
            barSize = (float)((troops - LEVEL8) / (LEVEL9 - LEVEL8));
        }
        else if (troops > LEVEL7)
        {
            level = 7;
            barSize = (float)((troops - LEVEL7) / (LEVEL8 - LEVEL7));
        }
        else if (troops > LEVEL6)
        {
            level = 6;
            barSize = (float)((troops - LEVEL6) / (LEVEL7 - LEVEL6));
        }
        else if (troops > LEVEL5)
        {
            level = 5;
            barSize = (float)((troops - LEVEL5) / (LEVEL6 - LEVEL5));
        }
        else if (troops > LEVEL4)
        {
            level = 4;
            barSize = (float)((troops - LEVEL4) / (LEVEL5 - LEVEL4));
        }
        else if (troops > LEVEL3)
        {
            level = 3;
            barSize = (float)((troops - LEVEL3) / (LEVEL4 - LEVEL3));
        }
        else if (troops > LEVEL2)
        {
            level = 2;
            barSize = (float)((troops - LEVEL2) / (LEVEL3 - LEVEL2));
        }
        else if (troops > 0)
        {
            level = 1;
            barSize = (float)(troops / LEVEL2);
        }
        else
        {
            level = 0;
            barSize = 0;
        }

        textMesh.text = level.ToString();
        Vector3 localScale = bar.transform.localScale;
        localScale.x = barSize;
        bar.transform.localScale = localScale;
    }



}