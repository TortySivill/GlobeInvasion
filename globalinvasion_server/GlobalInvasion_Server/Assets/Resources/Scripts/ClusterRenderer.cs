using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.IO; 
using System;

public class ClusterRenderer : MonoBehaviour {
	private CameraRig m_Rig;
	public float separation;
	public float convergence;
	
	private int index;

	private string GetCmdArguments(string arg) {
		string[] arguments = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < arguments.Length; i++) {
			if (arguments[i] == arg) {
				if (i+1 < arguments.Length)
					return arguments[i+1];
			}
		}
		// default to null
		return null;
	}

	public int GetCameraIndex ()
	{
		// load from command line
		string cmdIndex = GetCmdArguments ("-client");
		// if there is an index given from the command line (slave node)
		if (cmdIndex != null)
			return int.Parse (cmdIndex);
		// otherwise we are the master node
		else
			return 0;
	}

	void Start() {
		Camera camera = this.gameObject.GetComponent<Camera>();
		m_Rig = new CameraRig(camera, separation, convergence, true);
		index = GetCameraIndex();
		m_Rig.SetupCamera(index);
	}

	void Update() {
		if (Input.GetKeyDown("space")) {
			m_Rig.ToggleConverge();
			m_Rig.SetupCamera(index);
		}

		if (Input.GetKeyDown("a")) {
			m_Rig.IncreaseSeparation(-0.002f);
			m_Rig.SetupCamera(index);
		}

		if (Input.GetKeyDown("d")) {
			m_Rig.IncreaseSeparation(0.002f);
			m_Rig.SetupCamera(index);
		}

		if (Input.GetKeyDown("w")) {
			m_Rig.IncreaseConvergence(0.25f);
			m_Rig.SetupCamera(index);
		}

		if (Input.GetKeyDown("s")) {
			m_Rig.IncreaseConvergence(-0.25f);
			m_Rig.SetupCamera(index);
		}
	}
}
