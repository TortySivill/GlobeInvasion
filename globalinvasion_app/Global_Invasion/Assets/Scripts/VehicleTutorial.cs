using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleTutorial : MonoBehaviour
{
    private int troops;
    private CountryTutorial target;
    private TeamTutorial owner;
    private CommanderTutorial cm;
    private StrategistTutorial sm;
    private CountryTutorial tempTarget;
    private List<CountryTutorial> pathList;

    private Color team_color;
    private float speed;

    private Vector3 targetPosition;
    private Vector3 normDirection;

    private int tutorial;

    void Start()
    {
        cm = CommanderTutorial.FindObjectOfType<CommanderTutorial>();
        sm = StrategistTutorial.FindObjectOfType<StrategistTutorial>();
    }

    public void initialize(int troops, CountryTutorial target, TeamTutorial owner, List<CountryTutorial> pathList, CountryTutorial origin, int tutorial)
    {
        this.troops = troops;
        this.target = target;
        this.owner = owner;
        this.pathList = pathList;
        this.tutorial = tutorial;
        team_color = owner.getColor();

        GetComponentInChildren<Renderer>().materials[1].color = team_color;

        tempTarget = pathList[0];
        targetPosition = tempTarget.transform.position;
        speed = 1;
        normDirection = (targetPosition - this.transform.localPosition).normalized;
        this.transform.rotation = Quaternion.LookRotation(normDirection);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = targetPosition - this.transform.localPosition;
        float distThisFrame = speed * Time.deltaTime;

        if (direction.magnitude < distThisFrame)
        {
            if (tempTarget == target)
            {
                if (tutorial == 0)
                {
                    cm.tankReachedTarget(this, troops, target, owner);
                }
                else
                {
                    sm.tankReachedTarget(this, troops, target, owner);
                }
            }
            else
            {
                pathList.Remove(pathList[0]);
                tempTarget = pathList[0];
                targetPosition = tempTarget.transform.position;
                normDirection = (targetPosition - this.transform.localPosition).normalized;
                this.transform.rotation = Quaternion.LookRotation(normDirection);
            }
        }
        else
        {
            transform.Translate(normDirection * distThisFrame, Space.World);
        }
    }
}
