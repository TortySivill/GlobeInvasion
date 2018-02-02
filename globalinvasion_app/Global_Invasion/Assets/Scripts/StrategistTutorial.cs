using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class StrategistTutorial : MonoBehaviour {
    float native_width = 1920;
    float native_height = 1080;
    private CountryTutorial[] allCountries;
    private TeamTutorial[] allTeams;

    public GameObject BombTutorialStrat;
    public GameObject Explosion;
    public GameObject PlaneTutorialStrat;
    public GameObject Tank;
    public GameObject Bridge_a;
    public GameObject Bridge_b;
    public GameObject Bridge_c;

    public GameObject Arrow1;
    public GameObject Arrow2;
    public GameObject Arrow3;
    public GameObject Arrow4;

    public GameObject moneyTextOriginal;
    public GameObject moneyTextBomb;
    public GameObject moneyTextBridge;
    public GameObject moneyTextTroops;

    public ShortestPathTutorial pathManager;
    public Accelerometer Accelerometer;



    private static double count;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle guiStyleButton = new GUIStyle();


    // Use this for initialization
    void Start () {

        count = 0;
        BombTutorialStrat = Resources.Load("BombStrat") as GameObject;
        Explosion = Resources.Load("Explosion") as GameObject;
        PlaneTutorialStrat = Resources.Load("PlaneStrat") as GameObject;
        Tank = Resources.Load("Tank") as GameObject;
        //Bridge_a = Resources.Load("bridge_a") as GameObject;
        //Bridge_b = Resources.Load("bridge_b") as GameObject;
        //Bridge_c = Resources.Load("bridge_c") as GameObject;

        moneyTextOriginal = GameObject.Find("moneyTextOriginal");
        moneyTextBomb = GameObject.Find("moneyTextBomb");
        moneyTextTroops = GameObject.Find("moneyTextTroops");
        moneyTextBridge = GameObject.Find("moneyTextBridge");



        Bridge_a.GetComponent<Renderer>().sharedMaterials[0].color = Color.blue;
        Bridge_b.GetComponent<Renderer>().sharedMaterials[2].color = Color.blue;
        Bridge_c.GetComponent<Renderer>().sharedMaterials[4].color = Color.blue;

        hideBridgeA();
        hideBridgeB();
        hideBridgeC();

        Arrow2.SetActive(false);
        Arrow3.SetActive(false);
        Arrow4.SetActive(false);

        moneyTextBomb.SetActive(false);
        moneyTextBridge.SetActive(false);
        moneyTextTroops.SetActive(false);



        pathManager = ShortestPathTutorial.FindObjectOfType<ShortestPathTutorial>();


        List<CountryTutorial> startingCountries = new List<CountryTutorial>();
        allCountries = FindObjectsOfType<CountryTutorial>();
        allTeams = FindObjectsOfType<TeamTutorial>();

        CountryTutorial startingCountry = getCountryById(1);
        TeamTutorial Player = getTeamById(0);
        startingCountry.setOwner(Player);

        CountryTutorial opponentCountry = getCountryById(11);
        TeamTutorial Opponent = getTeamById(1);
        opponentCountry.setOwner(Opponent);


        Player.instantiateCursor(startingCountry);
        StartCoroutine(IndicatorFlasher(Player.Cursor));

    }

    IEnumerator IndicatorFlasher(GameObject current)
    {
        while (true)
        {
            current.GetComponent<MeshRenderer>().materials[0].color = Color.blue;
            yield return new WaitForSeconds(0.1f);
            current.GetComponent<MeshRenderer>().materials[0].color = Color.black;
            yield return new WaitForSeconds(.1f);
        }
    }

    void DoWindow0(int windowId) {

        GUILayout.Label("Your position is shown by the cursor which you can control using the arrows on your controller. Try to navigate to the country shown by the arrow.", guiStyle);
        //Arrow1.SetActive(true);
    }

    void DoWindow1(int windowId)
    {
      //Arrow1.SetActive(false);
        GUILayout.Label("Now you can drop a bomb on your opponent by navigating your cursor to their country and selecting the bomb button on your controller.", guiStyle);
    }


    void DoWindow2(int windowId)
    {
        GUILayout.Label("Nice one! A bomb destroys 25 troops but costs 250. You can see how much money you have at the bottom of your screen. Look! Your commander is trying to invade overseas but they need you to build a bridge. Navigate back to the port country shown by the arrow.", guiStyle);
        commanderMove();
    }

    void DoWindow3(int windowId)
    {
        GUILayout.Label("Now shake your phone to build the 3 stages of the bridge so your commander can move across.", guiStyle);
    }

    void DoWindow4(int windowId)
    {
        GUILayout.Label("Bridge 1 built! Keep shaking!", guiStyle);
    }

    void DoWindow5(int windowId)
    {
        GUILayout.Label("Look out! Your opponent has more troops than you, increase your team's troops by selecting the troops button on your controller whilst on your chosen country. Increasing troops costs 250.", guiStyle);
    }

    void DoWindow6(int windowId)
    {
        GUILayout.Label("Nice job! You are now ready to play the real game.", guiStyle);

    }


    void DoWindow15(int windowId)
    {
        GUILayout.Label("Bridge 1 built!", guiStyle);
    }


    void DoWindow16(int windowId)
    {
        GUILayout.Label("Bridge 2 built! Keep shaking!", guiStyle);
    }


    void DoWindow17(int windowId)
    {
        GUILayout.Label("Bridge 3 built! Bridge is now complete.", guiStyle);
    }

    void DoWindow30(int windowId)
    {
        GUILayout.Label("Nice one! A bomb destroys 25 troops but costs 250. Your money is shown above your controller. Look! Your commander is trying to invade overseas but they need you to build a bridge. Navigate back to the port country shown by the arrow.", guiStyle);
    }

    void OnGUI()
    {
        GUIStyle customButton = new GUIStyle("button");
        float rx =  Screen.width / native_width;
        float ry = Screen.height / native_height;
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(rx, ry, 1));
        customButton.fontSize = 40;
        guiStyle.fontSize = 40;
        guiStyle.wordWrap = true;
        guiStyle.normal.textColor = Color.white;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.contentOffset = new Vector2(0, 40);

        guiStyleButton.fontSize = 40;

        double doWindow0 = count;

        if (doWindow0 == 0 || doWindow0 == 0.5)
        {
            GUI.Window(0,new Rect(20, 10, 650, 450), DoWindow0, "");
        }
        if (doWindow0 == 1 || doWindow0 == 1.5)
        {
            GUI.Window(1, new Rect(20, 10, 650, 450), DoWindow1, "");
           
        }
        if (doWindow0 == 2)
        {
            GUI.Window(2, new Rect(20, 10, 650, 450), DoWindow2, "");
        }
        if (doWindow0 == 3)
        {
            GUI.Window(3, new Rect(20, 10, 650, 450), DoWindow3, "");

        }
        if (doWindow0 == 4)
        {
            GUI.Window(4, new Rect(20, 10, 650, 450), DoWindow4, "");

        }
        if (doWindow0 == 5 || doWindow0 == 5.5)
        {
            GUI.Window(5, new Rect(20, 10, 650, 450), DoWindow5, "");

        }

        if (doWindow0 == 6)
        {
            GUI.Window(6, new Rect(20, 10, 650, 450), DoWindow6, "");
            if (GUI.Button(new Rect(100, 200, 250, 75), "Home", customButton))
            {

                SceneManager.LoadScene("Scenes/TutorialStart");
                count = 0;
            }

            if (GUI.Button(new Rect(360, 200, 250, 75), "Start Game", customButton))
            {

                SceneManager.LoadScene("Scenes/Start");
                count = 0;
            }
            if (GUI.Button(new Rect(180, 290, 400, 75), "Commander Tutorial", customButton))
            {
                SceneManager.LoadScene("Scenes/CommanderTutorial");
                count = 0;
            }


        }

        if (doWindow0 == 15)
        {
            GUI.Window(15, new Rect(20, 10, 650, 450), DoWindow15, "");
        }

        if (doWindow0 == 16)
        {
            GUI.Window(16, new Rect(20, 10, 650, 450), DoWindow16, "");
        }

        if (doWindow0 == 17)
        {
            GUI.Window(17, new Rect(20, 10, 650, 450), DoWindow17 , "");
        }
        if (doWindow0 == 30 || doWindow0 == 2.5)
        {
            GUI.Window(30, new Rect(20, 10, 650, 450), DoWindow30, "");
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


    public TeamTutorial getTeamById(int t)
    {
        for (int ii = 0; ii < allTeams.Length; ii++)
        {
            if (allTeams[ii].id == t)
                return allTeams[ii];
        }
        return null;
    }


    public void manageButton(int i)
    {
        if (i == 4 && count == 1.5 && getTeamById(0).stratCurrent.id == 11)
        {
            initializePlane();
        }
        else if ( i == 4 && (count != 1 || getTeamById(0).stratCurrent.id != 11))
        {
            Handheld.Vibrate();
        }
        else if (i == 5 && count == 3)
        {
            showBridgeA();
            count = 4;
        }
        else if (i == 5 && count != 3)
        {
            Handheld.Vibrate();
        }
        else if (i == 5 && count == 4)
        {
           Accelerometer myaccelerometer;
           myaccelerometer = Instantiate(Accelerometer);     

        }
        else if (i == 6 && (count == 5 || count == 5.5 ) && getTeamById(0).stratCurrent.id == 12)
        {
            getTeamById(0).stratCurrent.troops = 30;
            getTeamById(0).stratCurrent.displayTroops();

            moneyTextBomb.SetActive(false);
            moneyTextTroops.SetActive(true);
            
    

            pathManager.addJob(getTeamById(0).stratCurrent, getCountryById(11), (int)getTeamById(0).stratCurrent.troops, getTeamById(0), 1);
            count = 6;

        }
        else if (i == 6 && (count != 5 && getTeamById(0).stratCurrent.id != 12))
        {
            Handheld.Vibrate();
        }
        else
        {
            CountryTutorial current = getTeamById(0).stratCurrent;
            CountryTutorial target = current.stratNeighbours[i];
            getTeamById(0).moveCursor(target);
        }

    }

    public void initializePlane()
    {
        Vector3 originPosition = new Vector3(30, 3.5f, 5);
        PlaneTutorialStrat myPlane;
        myPlane = Instantiate(PlaneTutorialStrat, originPosition, new Quaternion()).GetComponent<PlaneTutorialStrat>();
        myPlane.initialize(getTeamById(0).stratCurrent, getTeamById(0));

        moneyTextOriginal.SetActive(false);
        moneyTextBomb.SetActive(true);
    }

    public void planeReachedTarget(PlaneTutorialStrat plane)
    {
        plane.gameObject.SetActive(false);
    }


    public void bombReachedTarget(BombTutorialStrat bomb, CountryTutorial target)
    {
        bomb.gameObject.SetActive(false);


        Instantiate(Explosion, target.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        target.setTroops(0);
        count = 2;
    }

    public void dropBomb(CountryTutorial target, TeamTutorial owner)
    {
        Vector3 originPosition = target.transform.position + new Vector3(0, 3.5f, 0);
        BombTutorialStrat myBomb;
        myBomb = Instantiate(BombTutorialStrat, originPosition, new Quaternion()).GetComponent<BombTutorialStrat>();
        myBomb.initialize(target, owner);
    }

    public void commanderMove()
    {
        pathManager.addJob(getCountryById(1), getCountryById(0), (int)getCountryById(1).troops, getTeamById(0), 1);
        count = 30;
        
    }

    public void instantiateTank(int troops, CountryTutorial target, TeamTutorial owner, CountryTutorial source, List<CountryTutorial> pathList)
    {
       
        Vector3 originPosition = source.transform.position + new Vector3(0, 1, 0);
        pathList.Remove(pathList[0]);
        VehicleTutorial myTank;
        myTank = Instantiate(Tank, originPosition, target.transform.rotation).GetComponent<VehicleTutorial>();
        myTank.initialize(troops, target, owner, pathList, source, 1);
    }

    public void tankReachedTarget(VehicleTutorial tank, int troops, CountryTutorial target, TeamTutorial owner)
    {
        tank.gameObject.SetActive(false);
        owner.attackCountry(troops, target);

        if (count == 17)
        {
            count = 5;
            getCountryById(11).troops = 25;
            getCountryById(11).displayTroops();
        }


    }

    public void buildBridge(int a) { 
        CountryTutorial current = getTeamById(0).stratCurrent;
        if (!checkBridgeC())
        {
            if (a == 1 && (count == 4 || count == 3))
            {
                showBridgeA();
                count = 4;

            }
            else if (a == 2 && !checkBridgeB())
            {

                hideBridgeA();
                showBridgeB();
                count = 16;

            }

            else if (a == 3 && !checkBridgeA())
            {
                hideBridgeB();
                showBridgeC();
                count = 17;

            }
            if (checkBridgeC())
            {
                pathManager.addJob(getCountryById(0), getCountryById(12), (int)getCountryById(0).troops, getTeamById(0), 1);

            }
        }

    }


    public bool checkBridgeA()
    {
        if (GameObject.Find("bridge_a").transform.localScale == new Vector3((float)0.342, (float)0.1142, (float)0.079))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool checkBridgeB()
    {
        if (GameObject.Find("bridge_b").transform.localScale == new Vector3((float)0.342, (float)0.1142, (float)0.079))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool checkBridgeC()
    {
        if (GameObject.Find("bridge_c").transform.localScale == new Vector3((float)0.342, (float)0.1142, (float)0.079))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void showBridgeA()
    {
        GameObject.Find("bridge_a").transform.localScale = new Vector3((float)0.342, (float)0.1142, (float)0.079);

    }

    public void showBridgeB()
    {
        GameObject.Find("bridge_b").transform.localScale = new Vector3((float)0.342, (float)0.1142, (float)0.079);

    }

    public void showBridgeC()
    {
        GameObject.Find("bridge_c").transform.localScale = new Vector3((float)0.342, (float)0.1142, (float)0.079);



    }

    public void hideBridgeA()
    {
        GameObject.Find("bridge_a").transform.localScale = new Vector3(0, 0, 0);

    }


    public void hideBridgeB()
    {
        GameObject.Find("bridge_b").transform.localScale = new Vector3(0, 0, 0);

    }


    public void hideBridgeC()
    {
        GameObject.Find("bridge_c").transform.localScale = new Vector3(0, 0, 0);
    }

   IEnumerator Bouncer(GameObject arrow, double a, double b)
    {
        while (count == a || count == b || count == 30)
        {
            arrow.transform.Translate(0, 0.5f, 0);
            yield return new WaitForSeconds(0.5f);
            arrow.transform.Translate(0, -0.5f, 0);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Update is called once per frame
    void Update () {
		if (getTeamById(0).stratCurrent.id == 12 && count == 0.5)
        {
            count = 1;
        }
        if(count == 30 || count == 2.5)
        {
            if(getTeamById(0).stratCurrent.id == 0)
            {
                count = 3;
            }
        }
        if (count == 1)
        {
            Arrow1.SetActive(false);
            Arrow2.SetActive(true);
            //StartCoroutine(Bouncer(Arrow2, 1, 1.5));
            count = 1.5;
        }
        if (count == 2 || count == 30)
        {
            Arrow3.SetActive(true);
           // StartCoroutine(Bouncer(Arrow3, 2, 2.5));
            count = 2.5;
            Arrow2.SetActive(false);
        }
        if (count == 3)
        {
            Arrow3.SetActive(false);
        }
        if (count == 5)
        {
            Arrow4.SetActive(true);
           // StartCoroutine(Bouncer(Arrow4, 5, 5.5));
            count = 5.5;
        }
        if (count == 6)
        {
            Arrow4.SetActive(false);
        }
        if(count == 0)
        {
            //StartCoroutine(Bouncer(Arrow1, 0, 0.5));
            count = 0.5;
        }
        

    }
}
