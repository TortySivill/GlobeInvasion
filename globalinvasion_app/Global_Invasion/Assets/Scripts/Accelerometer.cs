
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    int n = 0;
    float value = 0;
    float shakeTimer = 2.0f;
    public GameManager gm;

    void Start()
    {
        gm = GameManager.safeFind<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.acceleration.magnitude > 2.0f)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0.0f)
            {
                gm.client.buildBridge(false);
                shakeTimer = 2.0f;
            }
        }
    }
}


