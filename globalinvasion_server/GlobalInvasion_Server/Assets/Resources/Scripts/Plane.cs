using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {
	private float   speed;
	private GameManager gm;
	private Team owner;
	private Country target;

	private Vector3 endPosition;
	private Vector3 targetPosition;
	private Vector3 normDirection;
	private Color team_color;
	private bool bomb_dropped;

	void Start () {
		gm = GameManager.safeFind<GameManager> ();

		GetComponentInChildren<Renderer> ().materials [3].color = new Color(0.5f, 0.5f, 0.5f);
	}

	public void initialize(Country target, Team owner) {
		this.target     = target;
		this.owner      = owner;
		team_color      = owner.getColor();

		targetPosition = target.transform.position + new Vector3 (0, 3.5f, 0);
		speed           		= 20;
		normDirection			= (targetPosition - this.transform.localPosition).normalized;
		endPosition             = transform.localPosition + 50 * normDirection;
		this.transform.rotation = Quaternion.LookRotation(normDirection);
		bomb_dropped = false;

		GetComponentInChildren<Renderer>().materials[6].color = team_color;
		GetComponentInChildren<Renderer>().materials[0].color = team_color;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 endDirection = endPosition - this.transform.localPosition;
		Vector3 targetDirection = targetPosition - this.transform.localPosition;
		float distThisFrame = speed * Time.deltaTime;

		if (!bomb_dropped && targetDirection.magnitude < distThisFrame) {
			normDirection = (endPosition - this.transform.localPosition).normalized;
			this.transform.rotation = Quaternion.LookRotation(normDirection);
			gm.dropBomb (target, owner);
			bomb_dropped = true;
			transform.Translate (normDirection * distThisFrame, Space.World);
		}
		else if (endDirection.magnitude < distThisFrame) {
			gm.planeReachedTarget (this);
		} else {
			transform.Translate (normDirection * distThisFrame, Space.World);
		}
	}
}
