using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortestPathTutorial : MonoBehaviour
{
    private CommanderTutorial cm;
    private StrategistTutorial sm;
    private List<PathJob> pathJobs = new List<PathJob>();

    void Start()
    {
        cm = CommanderTutorial.FindObjectOfType<CommanderTutorial>();
        sm = StrategistTutorial.FindObjectOfType<StrategistTutorial>();


    }

    public void addJob(CountryTutorial source, CountryTutorial target, int troops, TeamTutorial owner, int tutorial)
    {
        PathJob job = new PathJob(source, target, troops, owner, tutorial);
        pathJobs.Add(job);
        Debug.Log("addJOb called");
    }

    void Update()
    {
        if (pathJobs.Count == 0)
            return;

        PathJob job = pathJobs[0];
        pathJobs.RemoveAt(0);

        List<CountryTutorial> alreadyChecked = new List<CountryTutorial>();

        Node currentNode = new Node(job.source, null);
        List<Node> openSet = new List<Node>();
        openSet.Add(currentNode);

        while (currentNode.countryTutorial != job.target && openSet.Count > 0)
        {
            currentNode = openSet[0];
            openSet.RemoveAt(0);
            alreadyChecked.Add(currentNode.countryTutorial);
            foreach (CountryTutorial c in currentNode.countryTutorial.getNeighbours())
            {
                if (alreadyChecked.Contains(c))
                    continue;
                if (c.getOwner() == job.owner || c == job.target)
                {
                    Node tempNode = new Node(c, currentNode);
                    openSet.Add(tempNode);
                }
            }
        }

        if (currentNode.countryTutorial == job.target)
        {
            List<CountryTutorial> pathList = new List<CountryTutorial>();
            pathList.Add(job.target);
            while (currentNode.countryTutorial != job.source)
            {
                pathList.Add(currentNode.parent.countryTutorial);
                currentNode = currentNode.parent;
            }
            pathList.Reverse();
            //gm.instantiateTank(troops, target, owner, source, pathList);
            //gm.instantiateTank(job.troops, job.target, job.owner, job.source, pathList);
            if (job.tutorial == 0)
            {
                cm.instantiateTank(job.troops, job.target, job.owner, job.source, pathList);
            }
            else
            {
                sm.instantiateTank(job.troops, job.target, job.owner, job.source, pathList);
            }
        }
        else
        {
            Debug.Log("No path found");
        }
    }

}

internal class Node
{
    public CountryTutorial countryTutorial;
    public Node parent;

    public Node() { }

    public Node(CountryTutorial inp_country, Node parent)
    {
        this.countryTutorial = inp_country;
        this.parent = parent;
    }
}

internal class PathJob
{
    public CountryTutorial source;
    public CountryTutorial target;
    public int troops;
    public TeamTutorial owner;
    public int tutorial;

    public PathJob(CountryTutorial source, CountryTutorial target, int troops, TeamTutorial owner, int tutorial)
    {
        this.source = source;
        this.target = target;
        this.troops = troops;
        this.owner = owner;
        this.tutorial = tutorial;
    }
}
