/* Bridge.cs */

using UnityEngine;

public class Bridge : MonoBehaviour {
    public Team owner;
	public bool invulnerable;
	public Country port1;
	public Country port2;

	private float invTimer;

    void Start() {
		invulnerable = false;
		invTimer = Time.fixedTime;
    }

    void Update() {
		if ((Time.fixedTime - invTimer) >= 5) {
			invulnerable = false;
		}
    }

	public void setInvulnerable() {
		invulnerable = true;
		invTimer = Time.fixedTime;
	}
}
