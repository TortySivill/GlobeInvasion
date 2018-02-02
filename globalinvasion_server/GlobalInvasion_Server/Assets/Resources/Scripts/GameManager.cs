/* GameManager.cs */

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


    public Server server;
    public Dictionary<int, role_struct> connectionMap = new Dictionary<int, role_struct>();
    public List<int> projectorConnectionIds = new List<int>();

    public GameObject Tank;
    public GameObject PlanePrefab;
    public GameObject BombPrefab;
    public GameObject Explosion;
    public GameObject TroopSparkles;

    private int gameTimeInMins;
    private List<team_id> activeTeams = new List<team_id>();
    private Country[] allCountries;
    private Team[] allTeams;
    private bool gameRunning;
    private bool gameEnded;
    private List<GameObject> tanks = new List<GameObject>();
    private List<GameObject> planes = new List<GameObject>();
    private List<GameObject> bombs = new List<GameObject>();
    private Finish finish;
    private Dictionary<team_id, int> final_scores = new Dictionary<team_id, int>();
    private team_id winner;
    private List<team_id> finishedTeams = new List<team_id>();
	private Dictionary<int, string> nameMap = new Dictionary<int, string>();

    public AudioClip bombSoundClip;
    public AudioClip planeSoundClip;
    public AudioClip troopSoundClip;
	public AudioClip finishedBridgeClip;

    private AudioSource soundSource;

    // Sound files
    private Music music;

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start() {
        // Initialise server
        Debug.Log("Starting server...");
        server = new Server(8888, 10, this);
        finish = new Finish();
        gameTimeInMins = 7;

        music = safeFind<Music>();

        SceneManager.sceneLoaded += onLoad;
        gameRunning = false;
        gameEnded = false;

        soundSource = GetComponent<AudioSource>();

        SceneManager.LoadScene("Scenes/StartScreen");
    }

    // Update is called once per frame
    void Update() {
        server.tick();
   

    }

    // Called every X seconds to update country resources
    public void updateCountries() {
        // Store array of troop counts keyed by Country ID to send to projections
        double[] troopCounts = new double[allCountries.Length];

        // Update values
        foreach (Country country in allCountries) {
            country.increaseResources();
            troopCounts[country.id] = country.getTroops();
        }

        // Send troop counts to projection
        foreach (int connectionId in projectorConnectionIds) {
            server.sendTroopCounts(troopCounts, connectionId);
        }


        foreach (team_id t in activeTeams) {
            Team team = getTeamById(t);
            if (getStrategist(team) != -1)
            {
                server.sendMoney(getStrategist(team), team.getMoneyAsInt());
            }

                
        }

        foreach (team_id t in activeTeams)
        {
            Team team = getTeamById(t);
            if (getStrategist(team) != -1)
            {

                server.sendDisplayScore(getStrategist(team), team.getTeamScore(), team.getName());
            }


            if (getCommander(team) != -1)
            {
                server.sendDisplayScore(getCommander(team), team.getTeamScore(), team.getName());
            }

        }
    }

    public void onLoad(Scene scene, LoadSceneMode mode) {
        if (scene.name == "StartScreen") {
            music.chooseTeam.Play();
        }
        if (scene.name == "TestProjection") {
            music.chooseTeam.Stop();
            music.background.time = 156;
            music.background.Play();

            // Find all teams and countries
            allTeams = FindObjectsOfType<Team>();
            allCountries = FindObjectsOfType<Country>();

            // add starting countries
            List<Country> startingCountries = new List<Country>();
            startingCountries.Add(getCountryById(1));
            startingCountries.Add(getCountryById(9));
            startingCountries.Add(getCountryById(10));
            startingCountries.Add(getCountryById(18));

            // Assign each team a starting country
            foreach (team_id t in activeTeams) {
                Team team = getTeamById(t);
                int startIndex = (int)(UnityEngine.Random.value * startingCountries.Count);

                //team.addCountry(startingCountries[startIndex]);

                setCountryOwner(t, startingCountries[startIndex], (int)startingCountries[startIndex].getTroops());

                if (getCommander(team) != -1)
                    server.initializeCommanderConnection(getCommander(team), team);
                if (getStrategist(team) != -1)
                    server.initializeStrategistConnection(getStrategist(team), team);
                startingCountries.RemoveAt(startIndex);

                List<Country> start = team.getCountries();
                team.instantiateCursor(start[0]);
                server.initializeCursorOnProjectors(team, start[0]);
            }

			setNames ();

            server.sendNotConnectedInTime();
            gameRunning = true;

            // Set repeating function to update country resources
            InvokeRepeating("updateCountries", 0.0f, 1.0f);
            //TODO: Invoke warning functions to tell time left

            //End game after 7 Minutes
            Invoke("gameOver", gameTimeInMins * 60);
            //Invoke ("gameOver", 5);
        }
        else if (scene.name == "EndScreen") {
          music.background.Stop ();
          music.endscreen.Play ();

          EndDisplay ed = safeFind<EndDisplay> ();
          ed.setWinner (getTeamById (winner));
          foreach(team_id tId in final_scores.Keys) {
            ed.setTeamScore (tId, final_scores [tId], getTeamById(tId).getName());
          }
          Invoke ("restartGame", 30);
        }
    }

	public void setNames() {
		foreach (int id in connectionMap.Keys) {
			Team myTeam = getTeamById (connectionMap [id].team_id);
			string newName = nameMap[id];
			if(connectionMap[id].role == role_id.COMMANDER)
				myTeam.setCommanderName (newName);
			else if(connectionMap[id].role == role_id.STRATEGIST)
				myTeam.setStrategistName (newName);
		}
	}

	public void insertName(int connectionId, string name) {
		nameMap [connectionId] = name;
	}

    public void setCountryOwner(team_id teamId, Country country, int remainingTroops) {
        // Change in game server logic
        if (teamId != team_id.DEFAULT)
            country.setOwner(getTeamById(teamId));

		checkBridgesConquered ();

        // Notify projectors of country owner change
        foreach (int connectionId in projectorConnectionIds) {
            Debug.Log("send changing country to Projector " + connectionId);
            server.conquerCountry(country.id, teamId, connectionId, remainingTroops);
        }
    }

    public bool getGameRunning() {
        return gameRunning;
    }

    public bool getGameEnded() {
        return gameEnded;
    }

    public void removeActiveTeam(team_id tid) {
        activeTeams.Remove(tid);
    }

    public Country getCountryById(int id) {
        foreach (Country c in allCountries) {
            if (c.getId() == id) {
                return c;
            }
        }

        return null;
    }

    public void assignToTeam(role_struct t_str, int connection_id) {
        connectionMap[connection_id] = t_str;
        if (!activeTeams.Contains(t_str.team_id)) {
            activeTeams.Add(t_str.team_id);
        }
        safeFind<StartDisplay>().activateRole(t_str);
    }

    public void roleReady(role_struct rs) {
        safeFind<StartDisplay>().roleReady(rs);
    }

    public int getCommander(Team team)
    {
        foreach (int i in connectionMap.Keys) {
            if (connectionMap[i].team_id == team.team_col && connectionMap[i].role == role_id.COMMANDER)
                return i;
        }
        return -1;
    }

    public int getStrategist(Team team)
    {
        foreach (int i in connectionMap.Keys) {
            if (connectionMap[i].team_id == team.team_col && connectionMap[i].role == role_id.STRATEGIST)
                return i;
        }
        return -1;
    }

    public Team getTeamById(team_id t) {
        for (int ii = 0; ii < allTeams.Length; ii++) {
            if (allTeams[ii].team_col == t)
                return allTeams[ii];
        }
        Debug.Log("There is no Team with this team_id!");
        return null;
    }

    public void instantiateTank(int troops, Country target, Team owner, Country origin, List<Country> pathListTank) {
        Vector3 originPosition = origin.transform.position;
        pathListTank.Remove(pathListTank[0]);
        Vehicle myTank;
        if (tanks.Count > 0) {
            myTank = tanks[0].GetComponent<Vehicle>();
            tanks.Remove(tanks[0]);

            myTank.gameObject.transform.position = originPosition;

            myTank.gameObject.SetActive(true);
        } else {
            myTank = Instantiate(Tank, originPosition, target.transform.rotation).GetComponent<Vehicle>();
        }
        myTank.initialize(troops, target, owner, pathListTank, origin);

        int[] fromId = new int[1];
        fromId[0] = origin.id;
        server.sendMovementToProjectors(fromId, target.id, owner.team_col, pathListTank);
    }

    public void instantiatePlane(Country target, Team owner) {
        Vector3 originPosition = new Vector3(30, 3.5f, 5);
        Plane myPlane;
        if (planes.Count > 0) {
            myPlane = planes[0].GetComponent<Plane>();
            planes.Remove(planes[0]);

            myPlane.gameObject.transform.position = originPosition;

            myPlane.gameObject.SetActive(true);
        } else {
            myPlane = Instantiate(PlanePrefab, originPosition, new Quaternion()).GetComponent<Plane>();
        }
        myPlane.initialize(target, owner);
        soundSource.PlayOneShot(planeSoundClip);
    }

    public void tankReachedTarget(Vehicle vehicle) {
        Country target = vehicle.getTarget();
        Team tankOwner = vehicle.getOwner();
        Team targetOwner = target.getOwner();

        if (tankOwner == targetOwner) {
            target.addTroops((double)vehicle.getTroops());

            foreach (int id in projectorConnectionIds) {
                server.conquerCountry(target.id, targetOwner.team_col, id, (int)target.getTroops());
            }

        } else {
            vehicle.getOwner().attackCountry(vehicle.getTroops(), vehicle.getTarget());
        }

        tanks.Add(vehicle.gameObject);

        vehicle.gameObject.SetActive(false);
        checkTeamOut(vehicle.getOwner());
        checkTeamWon(vehicle.getTarget().getOwner());
    }

    public void planeReachedTarget(Plane plane) {
        planes.Add(plane.gameObject);
        plane.gameObject.SetActive(false);
    }

    public void bombReachedTarget(Bomb bomb) {
        Country target = bomb.getTarget();
        bombs.Add(bomb.gameObject);
        bomb.gameObject.SetActive(false);

        Debug.Log("bomb has reached it's target " + target.name);

        soundSource.PlayOneShot(bombSoundClip);
        Instantiate(Explosion, target.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

        if (target.getTroops() < 25) {
            target.setTroops(0);
        } else {
            target.removeTroops(25);
        }

        // Damage nearby bridge
        if (target.port && target.Bridge_a.GetComponent<Bridge>().owner != bomb.owner)
        {
            int level = target.checkBridgeLevel();
            Team owner = target.Bridge_a.GetComponent<Bridge>().owner;

            if (level == 3 && !target.Bridge_c.GetComponent<Bridge>().invulnerable)
            {
                Color bridgeColor = target.Bridge_c.GetComponent<Renderer>().materials[4].color;
                target.hideBridgeC();
                target.showBridgeB(bridgeColor);
                server.sendBridgeToProjectors(owner.team_col, target.id, 2);
            }
            else if (level == 2 && !target.Bridge_b.GetComponent<Bridge>().invulnerable)
            {
                Color bridgeColor = target.Bridge_b.GetComponent<Renderer>().materials[2].color;
                target.hideBridgeB();
                target.showBridgeA(bridgeColor);
                server.sendBridgeToProjectors(owner.team_col, target.id, 1);
            }
            else if (level == 1 && !target.Bridge_a.GetComponent<Bridge>().invulnerable)
            {
                target.hideBridgeA();
                server.sendBridgeToProjectors(owner.team_col, target.id, 0);

                target.Bridge_a.GetComponent<Bridge>().owner = null;
                target.Bridge_b.GetComponent<Bridge>().owner = null;
                target.Bridge_c.GetComponent<Bridge>().owner = null;
            }

			foreach (team_id tId in activeTeams) {
				checkStrategistOnCountry (target, getTeamById(tId));
			}
        }
    }

	public void checkStrategistOnCountry(Country c, Team stratTeam) {
		if (!c.port || !stratTeam)
			return;
		
		Country current = stratTeam.getCursorCountry ();

		if (current == c.Bridge_a.GetComponent<Bridge> ().port1
			|| current == c.Bridge_a.GetComponent<Bridge> ().port2) {
			//Debug.Log ("From checkStrat: " + bridgeBuildingPossible (stratTeam, c));
			server.sendBridgeCheck (getStrategist (stratTeam), bridgeBuildingPossible (stratTeam, c));
		}
	}

	public void checkBridgesConquered() {
		foreach (Bridge b in FindObjectsOfType<Bridge>()) {
			if (b.port1.getOwner () == b.port2.getOwner () && b.port1.getOwner () != b.owner) {
				setBridgeOwner (b.port1, b.port1.getOwner ());
				int level = b.port1.checkBridgeLevel ();
				if (level == 1) {
					b.port1.showBridgeA (b.owner.getColor ());
					server.sendBridgeToProjectors(b.owner.team_col, b.port1.id, 1);
				}
				else if (level == 2) {
					b.port1.showBridgeB (b.owner.getColor ());
					server.sendBridgeToProjectors(b.owner.team_col, b.port1.id, 2);
				}
				if (level == 3) {
					b.port1.showBridgeC (b.owner.getColor ());
					server.sendBridgeToProjectors(b.owner.team_col, b.port1.id, 3);
				}
			}
		}
	}

	public void setBridgeOwner(Country port, Team newOwner) {
		port.Bridge_a.GetComponent<Bridge> ().owner = newOwner;
		port.Bridge_b.GetComponent<Bridge> ().owner = newOwner;
		port.Bridge_c.GetComponent<Bridge> ().owner = newOwner;
	}

    public void dropBomb(Country target, Team owner) {
        Vector3 originPosition = target.transform.position + new Vector3(0, 3.5f, 0);
        Bomb myBomb;
        if (bombs.Count > 0) {
            myBomb = bombs[0].GetComponent<Bomb>();
            bombs.Remove(bombs[0]);

            myBomb.gameObject.transform.position = originPosition;

            myBomb.gameObject.SetActive(true);
        } else {
            myBomb = Instantiate(BombPrefab, originPosition, new Quaternion()).GetComponent<Bomb>();
        }
        myBomb.initialize(target, owner);
    }

    public void instantiateSparkles(Country c) {
        Instantiate(TroopSparkles, c.transform.position, Quaternion.identity);
        soundSource.PlayOneShot(troopSoundClip);
        server.sendSparklesToProjectors(c);
    }

    public void startGame() {
		server.setServerReady ();
        foreach (int i in projectorConnectionIds) {
            Debug.Log("Send start game to projectors");
            server.sendStartGame(i);
        }
        foreach (int i in connectionMap.Keys) {
            Debug.Log("Send start game to phones");
            server.sendStartGame(i);
        }
    }

    public void startGameScene() {
        SceneManager.LoadScene("Scenes/TestProjection");
    }

    public void freeRoleOfClient(int clientId) {
        team_id id = connectionMap[clientId].team_id;
        server.sendFreedRoleToProjectors(connectionMap[clientId]);
        safeFind<StartDisplay>().deactivateRole(connectionMap[clientId]);
        connectionMap.Remove(clientId);
        bool team_out = true;
        foreach (role_struct rs in connectionMap.Values) {
            if (rs.team_id == id) {
                team_out = false;
            }
        }
        if (team_out) {
            removeActiveTeam(id);
        }
    }

    public int troopsInTanks(Team team) {
        int troops = 0;
        foreach (Vehicle v in FindObjectsOfType<Vehicle>()) {
            if (v.getOwner() == team && !tanks.Contains(v.gameObject)) {
                troops += v.getTroops();
            }
        }
        return troops;
    }

    public void checkTeamOut(Team team) {
        if (finishedTeams.Contains(team.team_col))
            return;

        if (team.getCountryCount() > 0)
            return;

        if (troopsInTanks(team) > 0)
            return;

        //The team has no countries and no tanks and is therefore out of the game
        activeTeams.Remove(team.team_col);
        finishedTeams.Add(team.team_col);

        int score = calculateScore(team);
        finish.writeScore(score, team.getName());
        sendTopTenToFinishedTeams();

        Debug.Log("Team " + team.team_col + " finished with " + score + "points!");

        if (activeTeams.Count == 1)
            gameOver();
    }

    public void checkTeamWon(Team team) {
        if (team == null)
            return;
        //You conquered the whole world
        if (team.getCountryCount() == allCountries.Length) {
            foreach (team_id tId in activeTeams) {
                if (tId == team.team_col) {
                    continue;
                }
                if (troopsInTanks(getTeamById(tId)) > 0) {
                    //There are still other teams alive
                    return;
                }
            }
            gameOver();
        }
    }

    void sendTopTenToFinishedTeams() {
        List<Finish.playerScore> topTen = finish.getTopTen();
        foreach (team_id tId in finishedTeams) {
            server.sendTopTen(getTeamById(tId), false, final_scores[tId], topTen);
        }
    }

    public void gameOver() {
		if (gameEnded)
			return;
        int maxScore = 0;
        team_id maxScoreTeam = team_id.DEFAULT;

        for (int ii = 0; ii < activeTeams.Count; ii++) {
            Team team = getTeamById(activeTeams[ii]);
            int score = calculateScore(team);
            finish.writeScore(score, team.getName());
            Debug.Log("Team " + activeTeams[ii] + " finished with " + score + "points!");

            if (score > maxScore) {
                maxScore = score;
                maxScoreTeam = activeTeams[ii];
            }
        }
        winner = maxScoreTeam;

        List<Finish.playerScore> topTen = finish.getTopTen();
        for (int ii = 0; ii < activeTeams.Count; ii++) {
            if (activeTeams[ii] == maxScoreTeam) {
                //the winner
                server.sendTopTen(getTeamById(activeTeams[ii]), true, final_scores[activeTeams[ii]], topTen);
            } else {
                server.sendTopTen(getTeamById(activeTeams[ii]), false, final_scores[activeTeams[ii]], topTen);
            }
        }
        sendTopTenToFinishedTeams();

        //Format scores to send them to projectors
        int[] scores = new int[4];
        string[] names = new string[4];
        for (int ii = 0; ii < 4; ii++) {
            scores[ii] = -1;
            names[ii] = "";
        }
        foreach (team_id tid in final_scores.Keys) {
            int index = (int)tid - 1;
            scores[index] = final_scores[tid];
            names[index] = getTeamById(tid).getName();
        }
        server.sendScoresToProjector(scores, winner, names);

        CancelInvoke("updateCountries");
        gameRunning = false;
        gameEnded = true;

        SceneManager.LoadScene("Scenes/EndScreen");
    }

    public void reconnectOnRunningGame(int newConnectionId, string name) {
        Team team = null;

        foreach (Team t in allTeams) {

            if (name == t.commanderName) {
                // Remove old connection map entry, add new entry
                role_struct role = connectionMap[getCommander(t)];
                connectionMap.Remove(getCommander(t));
				if(connectionMap.ContainsKey(newConnectionId)) {
					connectionMap [getCommander (t)] = connectionMap [newConnectionId];
				}
                connectionMap[newConnectionId] = role;
                team = t;
            }

            else if (name == t.strategistName) {
                // Remove old connection map entry, add new entry
                role_struct role = connectionMap[getStrategist(t)];
                connectionMap.Remove(getStrategist(t));
				if(connectionMap.ContainsKey(newConnectionId)) {
					connectionMap [getStrategist (t)] = connectionMap [newConnectionId];
				}
                connectionMap[newConnectionId] = role;
                team = t;
            }
        }

        if (team == null) {
            Debug.Log("Couldn't find '" + name + "' in connection map! Declining...");
            server.declineClient(newConnectionId);
            return;
        }

        if (!finishedTeams.Contains(team.team_col)) {
            Debug.Log("Name '" + name + "' reconnecting to team '" + team.team_col + "'.");
            server.sendReconnectInformation(newConnectionId);
            return;
        }

        List<Finish.playerScore> topTen = finish.getTopTen();
        server.sendTopTen(team, false, final_scores[team.team_col], topTen);
    }

    public void reconnectAfterGameEnded(int connectionId) {
        List<Finish.playerScore> topTen = finish.getTopTen();
        team_id teamId = connectionMap[connectionId].team_id;
        if (teamId == winner) {
            server.sendTopTen(getTeamById(teamId), true, final_scores[teamId], topTen);
        } else {
            server.sendTopTen(getTeamById(teamId), false, final_scores[teamId], topTen);
        }
    }

    public int calculateScore(Team team) {
        double money_score = 0.1 * team.getMoneyAsInt();
        int score = team.getTroopCount() + (int)money_score;
        final_scores[team.team_col] = score;
        return score;
    }

    // Rotate cursors if they are on top of eachother
    public void checkCursors() {
        Cursor[] cursors = FindObjectsOfType<Cursor>();
        List<Cursor> done = new List<Cursor>();
        for (int i = 0; i < cursors.Length; i++) {
            if (!done.Contains(cursors[i])) {
                List<Cursor> onCurrent = new List<Cursor>();
                for (var j = 0; j < cursors.Length; j++) {
                    if (cursors[i].current == cursors[j].current) {
                        onCurrent.Add(cursors[j]);
                        done.Add(cursors[j]);
                    }
                } if (onCurrent.Count <= 1) {
                    onCurrent[0].rotate(0);
                } else if (onCurrent.Count == 2) {
                    onCurrent[0].rotate(0);
                    onCurrent[1].rotate(90);
                } else if (onCurrent.Count == 3) {
                    onCurrent[0].rotate(0);
                    onCurrent[1].rotate(90);
                    onCurrent[2].rotate(45);
                } else if (onCurrent.Count == 4) {
                    onCurrent[0].rotate(0);
                    onCurrent[1].rotate(90);
                    onCurrent[2].rotate(45);
                    onCurrent[3].rotate(135);
                }
            }
        }
    }

    public static T safeFind<T>() where T : Object {
        T[] objects = FindObjectsOfType<T>();
        if (objects.Length > 1) {
            Debug.Log("This should never happen, there should be exactly one object of type " + typeof(T));
        }
        else if (objects.Length == 0) {
            Debug.Log("There is no object of the type " + typeof(T));
            return null;
        }
        return objects[0];
    }

    public void restartGame() {
        Debug.Log("Restart the Game");
        server.reset();
        server.sendRestartGame();
        reset();
        SceneManager.LoadScene("Scenes/StartScreen");
    }

    void reset() {
        foreach (int i in connectionMap.Keys) {
            server.addUnsassignedClient(i);

			if (connectionMap [i].role == role_id.COMMANDER)
				nameMap [i] = getTeamById (connectionMap [i].team_id).getCommanderName ();
			if (connectionMap [i].role == role_id.STRATEGIST)
				nameMap [i] = getTeamById (connectionMap [i].team_id).getStrategistName ();
        }

        connectionMap.Clear();
        activeTeams.Clear();
        finishedTeams.Clear();
        gameRunning = false;
        gameEnded = false;

        tanks.Clear();
        planes.Clear();
        bombs.Clear();

        final_scores.Clear();
        winner = team_id.DEFAULT;

        CancelInvoke("gameOver");
    }

    public int getGameTime()
    {
        return gameTimeInMins;
    }

	public void playBridgeFininshed() {
		soundSource.PlayOneShot (finishedBridgeClip);
	}

    public List<team_id> getActiveTeams()
    {
        return activeTeams;
    }

	public List<team_id> getFinishedTeams()
	{
		return finishedTeams;
	}

	public bool bridgeBuildingPossible(Team team, Country port) {
		if (!port.port)
			return false;

		if (port.Bridge_a.GetComponent<Bridge> ().port1.getOwner () != team
		   && port.Bridge_a.GetComponent<Bridge> ().port2.getOwner () != team) {
			return false;
		}

		int level = port.checkBridgeLevel();
		if (level == 0) {
			return true;
		} 
		else if (level == 1 && port.Bridge_a.GetComponent<Bridge>().owner == team) {
			return true;
		}
		else if (level == 2 && port.Bridge_b.GetComponent<Bridge>().owner == team) {
			return true;
		}
		else {
			return false;
		}
	}
}
