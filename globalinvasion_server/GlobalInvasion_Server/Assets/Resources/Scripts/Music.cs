using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
	public AudioSource chooseTeam;
	public AudioSource endscreen;
	public AudioSource background;

	void Awake () {
		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
