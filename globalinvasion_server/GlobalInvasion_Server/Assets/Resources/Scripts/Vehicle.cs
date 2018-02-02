using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {
	private int     troops;
	private Country target;
	private Team    owner;
	private GameManager gm;
	private Country tempTarget;
	private List<Country> pathList;

	private Color   team_color;
	private float   speed;

	private Vector3 targetPosition;
	private Vector3 normDirection;

	void Start() {
		gm = GameManager.safeFind<GameManager>();
	}

	public void initialize(int troops, Country target, Team owner, List<Country> pathList, Country origin) {
		this.troops = troops;
		this.target = target;
		this.owner = owner;
		this.pathList = pathList;
		team_color = owner.getColor ();

		GetComponentInChildren<Renderer>().materials[1].color = team_color;

		tempTarget = pathList [0];
		targetPosition  		= tempTarget.transform.position;
		speed           		= 1;
		normDirection			= (targetPosition - this.transform.localPosition).normalized;
		this.transform.rotation = Quaternion.LookRotation (normDirection);
	}

	// Update is called once per frame
	void Update () {
		Vector3 direction = targetPosition - this.transform.localPosition;
		float distThisFrame = speed * Time.deltaTime;

		if (direction.magnitude < distThisFrame) {
			if (tempTarget == target) {
				gm.tankReachedTarget (this);
				GetComponent<AudioSource> ().Stop ();
			}
			else {
				pathList.Remove (pathList [0]);
				tempTarget = pathList [0];
				targetPosition = tempTarget.transform.position;
				normDirection = (targetPosition - this.transform.localPosition).normalized;
				this.transform.rotation = Quaternion.LookRotation (normDirection);
			}
		} else {
			transform.Translate (normDirection * distThisFrame, Space.World);
		}
	}

	public int getTroops() {
		return troops;
	}

	public Country getTarget() {
		return target;
	}

	public Team getOwner() {
		return owner;
	}
}
