﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTutorial : MonoBehaviour
{
    private CommanderTutorial cm;
    private CountryTutorial target;

    private float speed;
    private Vector3 direction;

    // Use this for initialization
    void Start()
    {
        cm = CommanderTutorial.FindObjectOfType<CommanderTutorial>();
    }

    public void initialize(CountryTutorial target, TeamTutorial owner)
    {
        this.target = target;

      GetComponentsInChildren<Renderer>()[4].materials[0].color = owner.getColor();
        direction = new Vector3(0, -1, 0);
        speed = 2;
    }

    // Update is called once per frame
    void Update()
    {
        float distThisFrame = speed * Time.deltaTime;
        if (transform.position.y <= 1)
        {
            cm.bombReachedTarget(this, target);
        }
        else
        {
            transform.Translate(direction * distThisFrame, Space.World);
        }
    }
}
