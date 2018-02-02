using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonTransitions : MonoBehaviour {
	private GameManager gm;
	public Text txt;
	public InputField IPinput;
	public InputField Portinput;
	public InputField TeamName;

	public 	Button 	enterButton;
	public 	Button 	screenButton;
	public  Button  planetariumButton;

	private bool reconnectClicked;


    void Start () 
	{
		gm = GameManager.safeFind<GameManager> ();

		if (screenButton != null)
			screenButton.onClick.AddListener (fillInfoScreen);

		if (planetariumButton != null)
			planetariumButton.onClick.AddListener(fillInfoPlanetarium);
			
		if (enterButton != null)
			enterButton.onClick.AddListener (delegate {Initialize ();});

		reconnectClicked = false;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			gm.onBackButton();
		}
	}

	void fillInfoScreen() {
		IPinput.text = "192.168.0.118";
		Portinput.text = "8888";
	}

	void fillInfoPlanetarium() {
		IPinput.text = "10.1.21.3";
		Portinput.text = "8888";
	}

	void Initialize() {
		if (IPinput.text != "" && Portinput.text != "" && TeamName.text != "") {
			showConnectingMessage();

			gm.setName (TeamName.text);
			string hostName = IPinput.text;
			int port = System.Int32.Parse(Portinput.text);

			gm.createClient (hostName, port);
		}
	}

	// Clicked the ready toggle
	public void onReadyToggle() {
		gm.client.sendReady();
	}

	// start button action
	public void onStartGame()
	{
		Debug.Log("You clicked on start game!");
		SceneManager.LoadScene("Scenes/ChooseTeam");
	}

	public void onRedTeam()
	{
		Debug.Log("You are the Red Team!");
		SceneManager.LoadScene("Scenes/ChoosePosition_red");

		gm.setTeamId(team_id.RED);
	}

	public void onBlueTeam()
	{
		Debug.Log("You are the Blue Team!");
		SceneManager.LoadScene("Scenes/ChoosePosition_blue");

		gm.setTeamId(team_id.BLUE);
	}

	public void onGreenTeam()
	{
		Debug.Log("You are the Green Team!");
		SceneManager.LoadScene("Scenes/ChoosePosition_green");

		gm.setTeamId(team_id.GREEN);

		//set team ID here
	}

	public void onYellowTeam()
	{
		Debug.Log("You are the Yellow Team!");
		SceneManager.LoadScene("Scenes/ChoosePosition_yellow");

		gm.setTeamId(team_id.YELLOW);

		//set team ID here
	}

	public void onMilitaryCommander()
	{
		gm.client.sendRole (gm.getTeamId(), role_id.COMMANDER);
		gm.setCommander ();

		Debug.Log("You are the military commander!");
	}

	public void onStrategist()
	{
		gm.client.sendRole (gm.getTeamId(), role_id.STRATEGIST);
		gm.setStrategist ();

		Debug.Log("You are the strategist!");
	}

	public void onHighscores() {
		SceneManager.LoadScene("Scenes/Highscores");
	}

	public static void toLoad()
	{
		SceneManager.LoadScene("Scenes/HoldingScreen");
	}

	public static void toStart()
	{
		SceneManager.LoadScene("Scenes/ChooseTeam");
	}

	public void showConnectingMessage()
	{
		enterButton.gameObject.SetActive(false);
		screenButton.gameObject.SetActive(false);
		planetariumButton.gameObject.SetActive(false);

		txt.gameObject.SetActive(true);
		IPinput.gameObject.SetActive(false);
		Portinput.gameObject.SetActive(false);
		TeamName.gameObject.SetActive (false);
	}

	public void showServerSelect()
	{
		enterButton.gameObject.SetActive(true);
		screenButton.gameObject.SetActive(true);
		planetariumButton.gameObject.SetActive(true);

		txt.gameObject.SetActive(false);
		IPinput.gameObject.SetActive(true);
		Portinput.gameObject.SetActive(true);
		TeamName.gameObject.SetActive (true);
	}

	public void disableConnectingMessage()
	{
		txt.gameObject.SetActive(false);
	}

	public void reconnect() {
		if(reconnectClicked)
			return;

		gm.client.disconnect();
		gm.client.connectToServer();
		reconnectClicked = true;
	}
}
