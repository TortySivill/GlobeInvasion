using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
	private GameManager gm;
	private Country target;
	public Team owner;

	private float speed;
	private Vector3 direction;

	// Use this for initialization
	void Start () {
		gm = GameManager.safeFind<GameManager>();
	}

	public void initialize(Country target, Team owner) {
		this.target = target;
		this.owner = owner;

		GetComponentsInChildren<Renderer>()[4].materials[0].color = owner.getColor();
		direction = new Vector3 (0, -1, 0);
		speed = 2;
	}
	
	// Update is called once per frame
	void Update () {
		float distThisFrame = speed * Time.deltaTime;
		if (transform.position.y <= 0) {
			gm.bombReachedTarget(this);
		} else {
			transform.Translate (direction * distThisFrame, Space.World);
		}
	}

	public Country getTarget() {
		return target;
	}
}
