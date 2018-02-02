using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CommanderTutorial : MonoBehaviour
{
    float native_width = 1920;
    float native_height = 1080;

    private CountryTutorial[] allCountries;
    private TeamTutorial[] allTeams;

    public ShortestPathTutorial pathManager;
    public GameObject Tank;
    public GameObject Bridge;
    public GameObject PlaneTutorial;
    public GameObject Explosion;
    public GameObject BombTutorial;

    public GameObject Arrow1;
    public GameObject Arrow2;
    public GameObject Arrow3;
    public GameObject Arrow4;
    public GameObject Arrow5;


    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    public Rect windowRect;
    private int doWindow0 = 1;
    private int doWindow1 = 0;
    private static int count = 0;

    private List<CountryTutorial> selectedNeighbours = new List<CountryTutorial>();


    bool sourceSelected1 = false;
    bool sourceSelectedSecond = false;
    bool sourceSelected2 = false;
    bool sourceSelected3 = false;
    bool sourceSelected4 = false;
    public Color flashColor = Color.white;
 

    public GameObject Indicator1;
    public GameObject Indicator2;
    public GameObject Indicator3;
    public GameObject Indicator4;
    public GameObject Indicator5;
    public GameObject Indicator6;

    private bool indicatorSelected = true;

    private static bool alreadyHit3 = false;
    private static bool alreadyHit6 = false;
    private static bool alreadyHit7 = false;
    private static bool alreadyHit10 = false;








    // Use this for initialization
    void Start()
    {
        GameObject.Find("bridge").transform.localScale = new Vector3(0, 0, 0);
        Tank = GameObject.Find("Tank");
        BombTutorial = Resources.Load("Bomb") as GameObject;
        Explosion = Resources.Load("Explosion") as GameObject;
        PlaneTutorial = GameObject.Find("PlaneStrat");


        Arrow1 = GameObject.Find("arrow1");
        Arrow2 = GameObject.Find("arrow2");
        Arrow3 = GameObject.Find("arrow3");
        Arrow4 = GameObject.Find("arrow4");
        Arrow5 = GameObject.Find("arrow5");


        Indicator1 = GameObject.Find("indicator1");
        Indicator2 = GameObject.Find("indicator2");
        Indicator3 = GameObject.Find("indicator3");
        Indicator4 = GameObject.Find("indicator4");
        Indicator5 = GameObject.Find("indicator5");
        Indicator6 = GameObject.Find("indicator6");



        Arrow2.SetActive(false);
        Arrow3.SetActive(false);
        Arrow4.SetActive(false);
        Arrow5.SetActive(false);

        Indicator1.SetActive(false);
        Indicator2.SetActive(false);
        Indicator3.SetActive(false);
        Indicator4.SetActive(false);
        Indicator5.SetActive(false);
        Indicator6.SetActive(false);




        List<CountryTutorial> startingCountries = new List<CountryTutorial>();
        allCountries = FindObjectsOfType<CountryTutorial>();
        allTeams = FindObjectsOfType<TeamTutorial>();

        CountryTutorial startingCountry = getCountryById(1);
        TeamTutorial Player = getTeamById(0);
        startingCountry.setOwner(Player);

        CountryTutorial winningCountry = getCountryById(12);
        winningCountry.setTroops(50);
        winningCountry.displayTroops();


        pathManager = ShortestPathTutorial.FindObjectOfType<ShortestPathTutorial>();



    }



    void DoWindow0(int windowID)
    {

        GUILayout.Box("Your countries are shown in BLUE. Try selecting the country shown by the arrow by clicking on it.", guiStyle);

    }

    void DoWindow1(int windowID)
    {
        GUILayout.Label("Nice job! To invade another country you must have enough troops! Do you have enough troops to invade your neighbour?", guiStyle);


    }

    void DoWindow2(int windowID)
    {
        GUILayout.Label("The number of troops is shown by a number and troop bar.", guiStyle);


    }

    void DoWindow3(int windowID)
    {
        GUILayout.Label("Great! Now you can try to invade by clicking on your own country and then the country you want to attack.", guiStyle);

    }

    void DoWindow4(int windowID)
    {
        GUILayout.Label("Nope, your troops are represented by a number. The higher the number, the more troops you have. The bar shows how close you are to the next troop level.", guiStyle);


    }

    void DoWindow5(int windowID)
    {
        GUILayout.Label("Nice Work! To move between ports you must get your strategist to build a bridge.", guiStyle);
      

    }

    void DoWindow6(int windowID)
    {
        GUILayout.Label("Nice Work! Now you're ready to invade overseas! But wait! This country has more troops than you. Try moving troops from your starting country. Do this by clicking on country 1 and LONG clicking on country 2.", guiStyle);

    }


    void DoWindow7(int windowID)
    {
        GUILayout.Label("You now have enough troops to conquer overseas - attack!", guiStyle);

    }


    void DoWindow8(int windowID)
    {
        GUILayout.Label("Your empire is growing fast! But look, an opponent is closing in.", guiStyle);
        initializeOpponent();


    }
    void DoWindow9(int windowID)
    {
        GUILayout.Label("Quick, before they invade, get your strategist to bomb them.", guiStyle);
       
    }

    void DoWindow10(int windowID)
    {
        GUILayout.Label("Nice! Now attack while they have no troops.", guiStyle);


    }

    void DoWindow11(int windowID)
    {
        GUILayout.Label("Uh oh a new opponent is getting close and they have so many troops! Select a country for your strategist to buy some troops.", guiStyle);
       


    }

    void DoWindow12(int windowID)
    {
        GUILayout.Label("Oh dear they still have so many more troops than you! Try attacking with multiple countries by DOUBLE clicking to select it's neighbours. Be warned: to attack a country you must own all the countries in it's path to the target.", guiStyle);

    }
    void DoWindow13(int windowId)
    {
        GUILayout.Label("Now select the country you want to attack.", guiStyle);
    }

    void DoWindow14(int windowID)
    {
        GUILayout.Label("Congratulations you're now ready to play the real game!", guiStyle);

    }


    void OnGUI()
    {
        GUIStyle customButton = new GUIStyle("button");

        customButton.fontSize = 40;

        float rx = Screen.width / native_width;
        float ry = Screen.height / native_height;
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));
        guiStyle.fontSize = 40;
        guiStyle.wordWrap = true;
        guiStyle.normal.textColor = Color.white;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.contentOffset = new Vector2(0, 100);

        int doWindow0 = count;

        if (doWindow0 == 0)
        {
            GUI.Window(0, new Rect(20, 10, 700, 600), DoWindow0, "");
        }
        else if (doWindow0 == 1)
        {
            GUI.Window(1, new Rect(20, 10, 700, 600), DoWindow1, "");
            if(GUI.Button(new Rect(260, 320, 100, 50), "Yes",customButton))
            {
                count = 3;
                Indicator1.SetActive(true);
                StartCoroutine(IndicatorFlasher(Indicator1));
                Indicator2.SetActive(true);
                StartCoroutine(IndicatorFlasher(Indicator2));
                Arrow1.SetActive(true);
                Arrow2.SetActive(true);
            }

            if (GUI.Button(new Rect(380, 320, 100, 50), "No",customButton))
            {
                count = 4;
            }
        }

        else if (doWindow0 == 3)
        {
            GUI.Window(3, new Rect(20, 10, 700, 600), DoWindow3, "");
        }

        else if (doWindow0 == 4)
        {
            GUI.Window(4, new Rect(20, 10, 700, 600), DoWindow4, "");
            if (GUI.Button(new Rect(260, 360, 200, 150), "Try again", customButton))
            {
                count = 1;
            }


        }

        else if (doWindow0 == 5)
        {
            GUI.Window(5, new Rect(20, 10, 700, 600), DoWindow5, "");
            if (GUI.Button(new Rect(250, 320, 250, 75), "Build Bridge", customButton))
            {
                buildBridge();
            }
        }

        else if (doWindow0 == 6)
        {
            GUI.Window(6, new Rect(20, 10, 700, 600), DoWindow6, "");
        }
        else if (doWindow0 == 7)
        {
            GUI.Window(7, new Rect(20, 10, 700, 600), DoWindow7, "");
        }

        else if (doWindow0 == 8)
        {
            GUI.Window(8, new Rect(20, 10, 700, 600), DoWindow8, "");
            if (GUI.Button(new Rect(250, 320, 250, 75), "Next", customButton))
            {
                count = 9;
            }

        }
        else if (doWindow0 == 9)
        {
            GUI.Window(9, new Rect(20, 10, 700, 600), DoWindow9, "");
            if (GUI.Button(new Rect(250, 320, 250, 75), "Drop Bomb", customButton))
            {
                count = 20;
                initializePlane();
            }

        }

        else if (doWindow0 == 10)
        {
            GUI.Window(10, new Rect(20, 10, 700, 600), DoWindow10, "");
        }
        else if (doWindow0 == 11)
        {
            GUI.Window(11, new Rect(20, 10, 700, 600), DoWindow11, "");

        }
        else if (doWindow0 == 12)
        {
            GUI.Window(12, new Rect(20, 10, 700, 600), DoWindow12, "");

        }
        else if (doWindow0 == 13)
        {
            GUI.Window(13, new Rect(20, 10, 700, 600), DoWindow13, "");

        }
        else if (doWindow0 == 14)
        {
              alreadyHit3 = false;
              alreadyHit6 = false;
              alreadyHit7 = false;
              alreadyHit10 = false;
              sourceSelected1 = false;
              sourceSelectedSecond = false;
              sourceSelected2 = false;
              sourceSelected3 = false;
              sourceSelected4 = false;


            GUI.Window(14, new Rect(20, 10, 700, 600), DoWindow14, "");
            if (GUI.Button(new Rect(100, 320, 250, 75), "Home", customButton))
            {

                SceneManager.LoadScene("Scenes/TutorialStart");
                count = 0;
            }

            if (GUI.Button(new Rect(360, 320, 250, 75), "Start Game", customButton))
            {

                SceneManager.LoadScene("Scenes/Start");
                count = 0;
            }
            if (GUI.Button(new Rect(210, 400, 350, 75), "Strategist Tutorial", customButton))
            {
                SceneManager.LoadScene("Scenes/StrategistTutorial");
                count = 0;
            }

        }




    }

    IEnumerator Flasher(CountryTutorial current)
    {
        while (sourceSelected1 || sourceSelected2 || sourceSelected3 || sourceSelected4)
        {
            current.GetComponent<MeshRenderer>().materials[0].color = Color.cyan;
            yield return new WaitForSeconds(.1f);
            current.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator IndicatorFlasher(GameObject current)
    {
        while (indicatorSelected)
        {
            current.GetComponent<SpriteRenderer>().materials[0].color = Color.cyan;
            yield return new WaitForSeconds(0.1f);
            current.GetComponent<SpriteRenderer>().materials[0].color = Color.white;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator SingleFlasher(CountryTutorial current)
    {
        for (int i = 0; i < 1; i++)
        {
            current.GetComponent<MeshRenderer>().materials[0].color = Color.cyan;
            yield return new WaitForSeconds(.1f);
            current.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            yield return new WaitForSeconds(.1f);
        }
    }


    public void manageClickEvent(CountryTutorial selected)
    {

        if (count == 0 && selected.id == 1)
        {
            StartCoroutine(SingleFlasher(selected));
            count = count + 1;
            Arrow1.SetActive(false);
        }
        else if(count == 0 && selected.id != 1)
        {
            Handheld.Vibrate();
        }
        else if (count == 3)
        {
            if (selected.id == 1 && !sourceSelected1)
            {
                sourceSelected1 = true;
                StartCoroutine(Flasher(selected));
                alreadyHit3 = true;
            }
            else if (selected.id == 0 && !sourceSelectedSecond)
            {
                sourceSelectedSecond = true;
                StartCoroutine(SingleFlasher(selected));
                pathManager.addJob(getCountryById(1), selected, (int)getCountryById(1).troops, getTeamById(0), 0);
                getCountryById(1).troops = 1;
                getCountryById(1).displayTroops();

            }
            else if(selected.id != 1 || selected.id != 0) {
                Handheld.Vibrate();

            }


        }
        else if (count == 6)
        {
            if (selected.id == 1 && alreadyHit6 == false )
            {
                sourceSelected4 = true;
                StartCoroutine(Flasher(selected));
                alreadyHit6 = true;
            }

            else if( count ==  6)
            {
                Handheld.Vibrate();
            }
        }

        else if (count == 7)
        {
            if (selected.id == 0 && alreadyHit7 == false)
            {
                sourceSelected2 = true;
                StartCoroutine(Flasher(selected));
                alreadyHit7 = true;
            }
            else if (selected.id == 12 && sourceSelected2)
            {
                sourceSelected2 = false;
                StartCoroutine(SingleFlasher(selected));
                pathManager.addJob(getCountryById(0), selected, (int)getCountryById(0).troops, getTeamById(0), 0);
                getCountryById(0).troops = 0;
                getCountryById(0).displayTroops();
            }
            else if (count == 7)
            {
                Handheld.Vibrate();
            }
        }
        else if (count == 10)
        {
            if (selected.id == 12 && alreadyHit10 == false)
            {
                sourceSelected3 = true;
                StartCoroutine(Flasher(selected));
                alreadyHit10 = true;
            }
            else if (selected.id == 15 && sourceSelected3)
            {
                sourceSelected3 = false;
                StartCoroutine(SingleFlasher(selected));
                pathManager.addJob(getCountryById(12), selected, (int)getCountryById(12).troops, getTeamById(0), 0);
                getCountryById(12).setTroops(0);
                getCountryById(12).displayTroops();
            }
            else
            {
                Handheld.Vibrate();
            }
        }

        else if (count == 11)
        {
            if (selected.id == 12)
            {
                StartCoroutine(SingleFlasher(selected));
                selected.troops = 50;
                selected.displayTroops();
                manageWindow();
            }
            else
            {
                Handheld.Vibrate();
            }
        }
        else if (count == 13)
        {
            if (selected.id == 14)
            {
                foreach (CountryTutorial c in selectedNeighbours)
                {
                    if (c.id != selected.id)
                    {
                        pathManager.addJob(c, selected, (int)c.troops, getTeamById(0), 0);
                        c.troops = 0;
                        c.displayTroops();
                    }
                }
                manageWindow();
            }
            else
            {
                Handheld.Vibrate();
            }
        }

    }

    public void manageLongClick(CountryTutorial selected)
    {
          if(count == 6 && sourceSelected4 == true)
        {
            sourceSelected4 = false;
            StartCoroutine(SingleFlasher(selected));
            pathManager.addJob(getCountryById(1), selected, (int)getCountryById(1).troops, getTeamById(0), 0);
            getCountryById(1).troops = 0;
            getCountryById(1).displayTroops();
        }
        else
        {
            Debug.Log("got here");
            Handheld.Vibrate();
        }
    }

    public void manageDoubleClickEvent(CountryTutorial selected)
    {
        if (count == 12 && selected.id == 12)
        {
            selectedNeighbours = selected.getNeighbours();
            selectedNeighbours.Add(selected);
            foreach (CountryTutorial c in selectedNeighbours)
            {
                if (c.getOwner() == null)
                {

                }
                else if (c.getOwner().id == 0)
                {
                    StartCoroutine(SingleFlasher(c));
                }
            }

            manageWindow();
        }
        else
        {
            Handheld.Vibrate();
        }

    }

    public CountryTutorial getCountryById(int id)
    {
        foreach (CountryTutorial c in allCountries)
        {
            if (c.getId() == id)
            {
                return c;
            }
        }

        return null;
    }

    public void instantiateTank(int troops, CountryTutorial target, TeamTutorial owner, CountryTutorial source, List<CountryTutorial> pathList)
    {
        

        Vector3 originPosition = source.transform.position;
        pathList.Remove(pathList[0]);
        VehicleTutorial myTank;
        myTank = Instantiate(Tank, originPosition, target.transform.rotation).GetComponent<VehicleTutorial>();
        myTank.initialize(troops, target, owner, pathList, source, 0);
    }

    public TeamTutorial getTeamById(int t)
    {
        for (int ii = 0; ii < allTeams.Length; ii++)
        {
            if (allTeams[ii].id == t)
                return allTeams[ii];
        }
        return null;
    }

    public void tankReachedTarget(VehicleTutorial tank, int troops, CountryTutorial target, TeamTutorial owner)
    {
        tank.gameObject.SetActive(false);
        Debug.Log("tank reached target");
        if (count == 6)
        {
            owner.moveTroops(troops, target);
            target.setTroops(55);
            target.displayTroops();
        }
        else
        {
            owner.attackCountry(troops, target);
        }
        manageWindow();

    }

    public void manageWindow()
    {
        if (count == 13)
        {
            count = 14;
            Arrow5.SetActive(false);
        }
        if (count == 12)
        {
            count = 13;
            Arrow3.SetActive(false);
            Arrow5.SetActive(true);
        }
        if (count == 11)
        {
            count = 12;
        }
        if (count == 10)
        {
            count = 11;
            Arrow3.SetActive(false);
            Arrow4.SetActive(false);
            Indicator5.SetActive(false);
            Indicator6.SetActive(false);
            Arrow3.SetActive(true);
            initializeOpponent();
        }
        if (count == 8)
        {
            count = 9;
        }
        if (count == 7)
        {
            count = 8;
            Arrow2.SetActive(false);
            Arrow3.SetActive(false);
            Indicator3.SetActive(false);
            Indicator4.SetActive(false);

        }
        if (count == 6)
        {
            count = 7;
            Arrow1.SetActive(false);
            Indicator2.SetActive(false);
            Indicator1.SetActive(false);

            Indicator3.SetActive(true);
            StartCoroutine(IndicatorFlasher(Indicator3));
            Indicator4.SetActive(true);
            StartCoroutine(IndicatorFlasher(Indicator4));
            Arrow3.SetActive(true);
        }
        if (count == 5)
        {
            count = 6;
            Arrow2.SetActive(true);
            Arrow1.SetActive(true);
            Indicator2.SetActive(true);
            Indicator1.SetActive(true);
        }
        if (count == 3)
        {
            count = 5;
            sourceSelected1 = false;
            sourceSelectedSecond = false;
            Arrow2.SetActive(false);
            Arrow1.SetActive(false);
            Indicator2.SetActive(false);
            Indicator1.SetActive(false);
        }


    }

    public void buildBridge()
    {
        GameObject.Find("bridge").GetComponent<Renderer>().sharedMaterials[4].color = getTeamById(0).color;
        GameObject.Find("bridge").transform.localScale = new Vector3((float)0.3, (float)0.2, (float)0.07);
        manageWindow();

    }

    public void initializeOpponent()
    {
        if (count == 11)
        {
            TeamTutorial Opponent = getTeamById(2);
            getCountryById(14).setOwner(Opponent);
            getCountryById(14).troops = 100;
            getCountryById(14).displayTroops();

        }
        else
        {


            CountryTutorial opponentCountry = getCountryById(15);
            TeamTutorial Opponent = getTeamById(1);
            opponentCountry.setOwner(Opponent);
          
        }


    }

    public void initializeNewOpponent()
    {
        CountryTutorial opponentCountry = getCountryById(16);
        TeamTutorial Opponent = getTeamById(2);
        opponentCountry.setOwner(Opponent);
        opponentCountry.troops = 100;
        opponentCountry.displayTroops();

    }


    public void initializePlane()
    {
        Vector3 originPosition = new Vector3(30, 3.5f, 5);
        PlaneTutorial myPlane;
        myPlane = Instantiate(PlaneTutorial, originPosition, new Quaternion()).GetComponent<PlaneTutorial>();
        myPlane.initialize(getCountryById(15), getTeamById(0));
    }

    public void planeReachedTarget(PlaneTutorial plane)
    {
        plane.gameObject.SetActive(false);
    }


    public void bombReachedTarget(BombTutorial bomb, CountryTutorial target)
    {
        bomb.gameObject.SetActive(false);

        Debug.Log("bomb has reached it's target " + target.name);

        Instantiate(Explosion, target.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        target.setTroops(0);
        count = 10;
        Arrow4.SetActive(true);
        Arrow3.SetActive(true);
        Indicator5.SetActive(true);
        StartCoroutine(IndicatorFlasher(Indicator5));
        Indicator6.SetActive(true);
        StartCoroutine(IndicatorFlasher(Indicator6));

    }

    public void dropBomb(CountryTutorial target, TeamTutorial owner)
    {
        Vector3 originPosition = target.transform.position + new Vector3(0, 3.5f, 0);
        BombTutorial myBomb;
        myBomb = Instantiate(BombTutorial, originPosition, new Quaternion()).GetComponent<BombTutorial>();
        Debug.Log("success");
        myBomb.initialize(target, owner);
    }
}