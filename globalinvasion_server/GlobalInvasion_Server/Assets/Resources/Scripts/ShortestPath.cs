using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortestPath : MonoBehaviour
{
    private GameManager gm;
    private List<PathJob> pathJobs = new List<PathJob>();

    void Start()
    {
        gm = GameManager.safeFind<GameManager>();
    }

    public void addJob(Country source, Country target, int troops, Team owner)
    {
        PathJob job = new PathJob(source, target, troops, owner);
        pathJobs.Add(job);
    }

    void Update()
    {
        if (pathJobs.Count == 0)
            return;

        PathJob job = pathJobs[0];
        pathJobs.RemoveAt(0);

        List<Country> alreadyChecked = new List<Country>();
        int g_score = 0;
        float f_score_origin = g_score + calculateH_score(job.source, job.target);
        Node currentNode = new Node(job.source, null, f_score_origin, g_score);
        List<Node> openSet = new List<Node>();
        openSet.Add(currentNode);

        while (currentNode.country != job.target && openSet.Count > 0)
        {
            currentNode = openSet[0];
            foreach (Node n in openSet)
            {
                if (n.f_score < currentNode.f_score)
                {
                    currentNode = n;
                }
            }
            openSet.Remove(currentNode);
            alreadyChecked.Add(currentNode.country);
            foreach (Country c in currentNode.country.getNeighbours())
            {
                if (alreadyChecked.Contains(c))
                    continue;
                if (c.getOwner() == job.owner || c == job.target)
                {
                    float f_score = calculateH_score(c, job.target) + currentNode.g_score + 1;
                    Node tempNode = new Node(c, currentNode, f_score, currentNode.g_score + 1);
                    openSet.Add(tempNode);
                }
            }
        }

        if (currentNode.country == job.target)
        {
            List<Country> pathList = new List<Country>();
            pathList.Add(job.target);
            while (currentNode.country != job.source)
            {
                pathList.Add(currentNode.parent.country);
                currentNode = currentNode.parent;
            }
            pathList.Reverse();
            //gm.instantiateTank(troops, target, owner, source, pathList);
            //gm.instantiateTank(job.troops, job.target, job.owner, job.source, pathList);
            job.owner.checkPath(job.troops, job.target, job.owner, job.source, pathList);
        }
        else
        {
            Debug.Log("No path found");
            gm.server.sendCommanderFeedback(gm.getCommander(job.owner), false, job.target.id);
        }
    }

    public float calculateH_score( Country source, Country target)
    {
        float dist = Vector3.Distance(source.transform.position, target.transform.position);
        return dist;
    }

}

internal class Node
{
    public Country country;
    public Node parent;
    public float f_score;
    public int g_score;

    public Node() { }

    public Node(Country inp_country, Node parent, float f_score, int g_score)
    {
        this.country = inp_country;
        this.parent = parent;
        this.f_score = f_score;
        this.g_score = g_score;
    }
}

internal class PathJob
{
    public Country source;
    public Country target;
    public int troops;
    public Team owner;

    public PathJob(Country source, Country target, int troops, Team owner)
    {
        this.source = source;
        this.target = target;
        this.troops = troops;
        this.owner = owner;
    }
}
