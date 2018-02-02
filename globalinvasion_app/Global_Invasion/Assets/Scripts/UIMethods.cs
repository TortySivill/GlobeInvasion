using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMethods : MonoBehaviour {
	public Text moneyText;
    Team active_team;
	public Text bridgeText;

	private AudioSource hammering;

    // Use this for initialization
    void Start () {
        active_team = GameManager.safeFind<Team> ();
		hammering = GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update () {
        displayMoney ();
    }

    void displayMoney() {
		int money = (int)active_team.getMoney();
		if (money > 999999) {
			moneyText.text = "Money: ∞";
		} else {
			moneyText.text = "Money: " + money;
		}
    }

	public void setBridgeText(bool display) {
		bridgeText.enabled = display;

		if (display && !hammering.isPlaying)
			hammering.Play();
		else
			hammering.Stop();
	}
}
