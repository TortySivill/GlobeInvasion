using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
	private Vector3 currentPosition;
	public Country current;
	private Team 	owner;
	private GameManager gm;

	private float oscillationStart = 0.15f;
	private float oscillationEnd   = 0.25f;
	private float oscillationSpeed = 5.0f;

	private Color 	team_color;

	public Cursor() {

	}

	// Use this for initialization
	void Start () {
		gm = GameManager.safeFind<GameManager>();
	}

	void Update() {
		// Make cursor bounce up and down
		Vector3 position = this.transform.position;
		position.y = oscillationStart + Mathf.Sin(oscillationSpeed * Time.time) * (oscillationEnd - oscillationStart);
		this.gameObject.transform.position = position;
	}

	public void initialize (Vector3 currentPosition, Country current, Team owner) {
		this.current	= current;
		this.owner 		= owner;
		team_color 		= owner.getColor();

		GetComponent<Renderer>().materials[1].color = team_color;

		currentPosition = this.transform.localPosition;
	}

	public void move (Country target) {
		if (target == current) {
			Debug.Log ("Invalid Move. Try Again.");
			gm.server.sendInvalidStratMovement (gm.getStrategist (owner));
		} else {
			this.transform.localPosition = target.transform.position + new Vector3 (0, 0.1f, 0);
			current = target;
			Debug.Log ("Moved to " + target);
			gm.checkCursors();
		}	
	}

	public void select() {
	
	}

	public Country getCurrent() {
		return current;
	}

	public void setCurrent(Country newCurrent) {
		current = newCurrent;
	}

	public Team getOwner() {
		return owner;
	}

	public void rotate(float angle) {
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
		this.gameObject.transform.rotation = rotation;
	}
		
}
