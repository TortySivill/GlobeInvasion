/* CameraRig.cs */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/* Stereoscopic camera rig for cluster rendering */
public class CameraRig {

	private Camera camera;

	private Vector3 centerPosition;
    private Quaternion initialRotation;

	private float separation;
	private float convergence;
	private bool converge;	

	public CameraRig(Camera camera, float separation, float convergence, bool converge) {
		Debug.Assert(camera);
		Debug.Assert(separation > 0);
		Debug.Assert(convergence > 0);

		this.centerPosition = camera.transform.position;
        this.initialRotation = camera.transform.rotation;

		this.camera      = camera;
		this.separation  = separation;
		this.convergence = convergence;
		this.converge    = converge;
	}

	public void IncreaseSeparation(float value) {
		this.separation += value;
	}

	public void IncreaseConvergence(float value) {
		this.convergence += value;
	}

	public void ToggleConverge() {
		converge = !converge;
	}

	public void SetupCamera(int cameraIndex) {
        camera.transform.rotation = initialRotation;

		/* Offset camera left (0) or right (1) and look at convergence point */
        Vector3 posConvergence = centerPosition + convergence * camera.transform.forward;
        
		/* Master=Right, Slave=Left */
		int direction = cameraIndex == 0 ? 1 : -1;
		
		camera.transform.position = centerPosition + direction * camera.transform.right * (separation / 2);

		if (converge) {
		    camera.transform.LookAt(posConvergence);
		} else {
			camera.transform.LookAt(camera.transform.position + convergence * camera.transform.forward);
		}
	}
}
