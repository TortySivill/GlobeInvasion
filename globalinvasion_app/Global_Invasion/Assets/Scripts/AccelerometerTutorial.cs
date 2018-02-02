using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerTutorial : MonoBehaviour
{
    int n = 1;
    float value = 0;
    float shakeTimer = 4.0f;
    public StrategistTutorial sm;

    void Start()
    {
        sm = StrategistTutorial.FindObjectOfType<StrategistTutorial>();
        //sm.buildBridge(0);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.acceleration.magnitude > 2.0f)
        {
            shakeTimer -= Time.deltaTime;
            if (System.Math.Round(shakeTimer, 1) == 3.0f && n == 1)
            {
                n++;
                sm.buildBridge(1);
            }
            if (System.Math.Round(shakeTimer, 1) == 2.0f && n == 2)
            {
                n++;
                sm.buildBridge(2);
            }
            else if (System.Math.Round(shakeTimer, 1) == 0.0f && n == 3)
            {
                n++;
                sm.buildBridge(3);
                this.gameObject.SetActive(false);
            }
        }


    }
}
