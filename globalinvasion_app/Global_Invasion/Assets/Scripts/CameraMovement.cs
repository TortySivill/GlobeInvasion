using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	public float orthoZoomSpeed ;        	// The rate of change of the orthographic size in orthographic mode.
	public float pan_speed;					// The rate of change for the dragging movement

	private float touch_time;
	private float double_click_time;
	private bool potential_double_click;
	private Country touched_country;
	private Team team;

	Camera cam;

	void Start() {
		cam = FindObjectOfType<Camera>();
		pan_speed = 0.1f;
		team = GameManager.safeFind<Team> ();
	}

	void Update()
	{
		// Drag camera to move
		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Moved) {
			Vector2 delta_pos = Input.GetTouch (0).deltaPosition;
			float zoom_factor = cam.orthographicSize / 15;
			Vector3 movement = new Vector3 (-delta_pos.x * pan_speed * zoom_factor, 0, -delta_pos.y * pan_speed * zoom_factor);
			//Debug.Log ("Trans: " + movement);
			cam.transform.Translate (movement, Space.World);

			Vector3 cam_pos = cam.transform.position;
			cam_pos.x = cam_pos.x < -9f ? -9f : cam_pos.x;
			cam_pos.x = cam_pos.x > 9f ? 9f : cam_pos.x;
			cam_pos.z = cam_pos.z < -10f ? -10f : cam_pos.z;
			cam_pos.z = cam_pos.z > 9f ? 9f : cam_pos.z;
			cam.transform.position = cam_pos;

			//If moved do not select country!
			if(delta_pos.magnitude > 2)
				touched_country = null;

		}
		//If something was clicked (mouse down + mouse up)
		else if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began) {
			//Debug.Log ("I clicked here: " + Input.GetTouch (0).position);
			Ray my_ray = Camera.main.ScreenPointToRay(Input.GetTouch (0).position);
			RaycastHit[] hits;
			hits = Physics.RaycastAll (my_ray);

			foreach (RaycastHit hit in hits) {
				Debug.Log("Hit something.");
				Country c = hit.collider.GetComponent<Country> ();
				if (c == touched_country && (Time.fixedTime - double_click_time) < 0.4) {
					potential_double_click = true;
					touch_time = Time.fixedTime;
				} else if (c) {
					touch_time = Time.fixedTime;
					touched_country = c;
				} else
					team.clearSelectedCountries();
			}

			if (hits.Length == 0) {
				Debug.Log("Hit nothing.");
				team.clearSelectedCountries();
			}
		}
		else if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			//Debug.Log ("I clicked here: " + Input.GetTouch (0).position);
			Ray my_ray = Camera.main.ScreenPointToRay(Input.GetTouch (0).position);
			RaycastHit[] hits;
			hits = Physics.RaycastAll (my_ray);

			foreach (RaycastHit hit in hits) {
				Country c = hit.collider.GetComponent<Country> ();
				if (c == touched_country) {
					if ((Time.fixedTime - touch_time) < 0.2) {
						if (potential_double_click) {
							c.onDoubleClick();
							double_click_time = 0;
						} else {
							c.onClick();
							double_click_time = Time.fixedTime;
						}
					} else {
						c.onLongClick();
					}
				}
			}
			potential_double_click = false;
		}
		// If there are two touches on the device...
		else if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// Change size of orthographic projection
			cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			cam.orthographicSize = cam.orthographicSize > 15 ? 15 : cam.orthographicSize;
			cam.orthographicSize = cam.orthographicSize < 5 ? 5 : cam.orthographicSize;
		}
	}
}
