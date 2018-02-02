using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public  Client	client;

	private Country[]	allCountries;
	public Team       	playerTeam;
	private GameObject 	moneyText;
	private string 		myName;

	private bool won;
	private int score;
	private List<Finish.playerScore> topTen;

	public team_id teamId;
	public role_id roleId;
    private int gameTimeInMinutes;

	void Awake() {
		DontDestroyOnLoad (this.gameObject);
	}

	void Start() {
		client = null;
		SceneManager.sceneLoaded += onLoad;
		teamId = team_id.DEFAULT;
		roleId = role_id.DEFAULT;

        gameTimeInMinutes = 7;
	}

	void Update() {
		if(client != null)
			client.tick();
	}


    public int getGameTime()
    {
        return gameTimeInMinutes;
    }

	// Reset GM state for restarting the game
	public void reset() {
		teamId = team_id.DEFAULT;
		roleId = role_id.DEFAULT;
		topTen.Clear();

		SceneManager.LoadScene("Scenes/ChooseTeam");
	}

	public void createClient(string hostName, int port) {
		if (client == null) {
			Debug.Log("Starting client...");
			client = new Client (hostName, port, this);
		}
		else {
			Debug.Log ("Connect to new server");
			client.setServerInfo (hostName, port);
			client.connectToServer ();
		}
	}

	public void onLoad(Scene scene, LoadSceneMode mode) {
		if (scene.name == "Testscene") {
			allCountries = FindObjectsOfType<Country> ();
			playerTeam = safeFind<Team> ();
			client.sendStartGame ();
		} else if (scene.name == "blueDPad" || scene.name == "redDPad"|| scene.name == "yellowDPad" || scene.name == "greenDPad") {
            playerTeam = safeFind<Team>();
            moneyText = FindObjectOfType<GameObject> ();
			client.sendStartGame ();

		} else if (scene.name == "ChooseTeam") {
			teamId = team_id.DEFAULT;
		} else if (scene.name == "EndScreen") {
			safeFind<Finish> ().Initialize(won);
		} else if (scene.name == "Highscores") {
			safeFind<Highscores>().Initialize(topTen);
		}
	}

	public void setName(string s)
	{
		myName = s;
	}
	public string getName()
	{
		return myName;
	}
		
	void backToServerSelect() {
		ButtonTransitions bt = safeFind<ButtonTransitions> ();
		bt.showServerSelect();
		if (client != null) {
			client.allowDisconnect ();
			client.disconnect ();
		}
	}

	public void onBackButton() {
		string sceneName = SceneManager.GetActiveScene().name;
		Debug.Log ("Back button pressed in " + sceneName);
		if (sceneName == "ChoosePosition_blue" ||
		    sceneName == "ChoosePosition_green" ||
		    sceneName == "ChoosePosition_red" ||
		    sceneName == "ChoosePosition_yellow") {
			SceneManager.LoadScene ("Scenes/ChooseTeam");
		} 
		else if (sceneName == "HoldingScreen") {
			client.freeRole ();
			SceneManager.LoadScene ("Scenes/ChooseTeam");
		} 
		else if (sceneName == "Start") {
			backToServerSelect ();
		} else if (sceneName == "Highscores") {
			SceneManager.LoadScene("Scenes/EndScreen");
		}
	}

	public void startGame() {
		if(roleId == role_id.COMMANDER)
			SceneManager.LoadScene("Scenes/Testscene");
		else if (roleId == role_id.STRATEGIST){
			switch(teamId){
			case team_id.BLUE:
				SceneManager.LoadScene ("Scenes/blueDPad");
				break;
			case team_id.GREEN:
				SceneManager.LoadScene ("Scenes/greenDPad");
				break;
			case team_id.YELLOW:
				SceneManager.LoadScene ("Scenes/yellowDPad");
				break;
			case team_id.RED:
				SceneManager.LoadScene ("Scenes/redDPad");
				break;
			}
		}
	}

	public void connected(bool successfull) {
		if (successfull) {
			SceneManager.LoadScene("Scenes/ChooseTeam");
		} else {
			SceneManager.LoadScene ("Scenes/GameRunningScreen");
		}
	}

	public void conquerCountry(int id) {
		Country c = getCountryByID(id);

		if (c.getOwner() == null) {
			c.setOwner(playerTeam);
		} else {
			playerTeam.checkSelectedAndRemove(c);
			c.setOwner(null);
		}
	}

	public Country getCountryByID(int id) {
		foreach (Country c in allCountries) {
			if (c.getId() == id) {
				return c;
			}
		}

		return null;
	}

	public team_id getTeamId() {
		return teamId;
	}

	public void setTeamId(team_id id) {
		teamId = id;
	}

	public void setStartingCountries(int[] ids, Color c) {
		Team    team         = safeFind<Team>();
		team.color           = c;

		GameManager.safeFind<UICommander>().setCommanderBackgroundColor(c);

		foreach (int id in ids) {
			Country startCountry = getCountryByID(id);
			startCountry.setOwner(team);
		}
	}

	public void setStratColor(Color c) {
		Team    team         = safeFind<Team>();
		team.color           = c;
	}

	public void disconnected() {
		SceneManager.LoadScene ("Scenes/DisconnectScreen");
	}

	public void connectionProblem() {
		SceneManager.LoadScene ("Scenes/ConnectionProblem");
	}

	public static T safeFind<T>() where T : Object {
		T[] objects = FindObjectsOfType<T> ();
		if (objects.Length > 1) {
			Debug.Log ("This should never happen, there should be exactly one object of type " + typeof(T));
		} 
		else if (objects.Length == 0) {
			Debug.Log ("There is no object of the type " + typeof(T));
			return null;
		}
		return objects [0];
	}

	public void setCommander() {
		roleId = role_id.COMMANDER;
	}

	public void setStrategist() {
		roleId = role_id.STRATEGIST;
	}

    public void deActivate(Accelerometer end)
    {
        end.gameObject.SetActive(false);
    }

    public void gameEnded(bool won, int score, List<Finish.playerScore> topTen)
	{
		this.won    = won;
		this.score  = score;
		this.topTen = topTen;

		if (SceneManager.GetActiveScene().name == "Highscores") {
			SceneManager.LoadScene("Highscores");
			return;
		}

		if (SceneManager.GetActiveScene().name != "EndScreen")
			SceneManager.LoadScene("EndScreen");
	}

}
