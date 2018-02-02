using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICommander : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void setCommanderBackgroundColor(Color c) {
		Color darkerColor = new Color (c.r - 0.7f, c.g - 0.7f, c.b - 0.7f);
		FindObjectOfType<Camera> ().backgroundColor = darkerColor;
	}
}
