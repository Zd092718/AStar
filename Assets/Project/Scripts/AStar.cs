using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds information about certain position
//, so it's used in a pathfinding algorithm.
class Node
{
    //Every node may have different values,
    //according to your game map
    public enum Value
    {
        FREE,
        BLOCKED
    }
    //Nodes have X and Y positions
    public int posX;
    public int posY;

    //gCost is basic cost to go from one node to the other.
	public int gCost = int.MaxValue;
    //hCost is a heuristic that estimates the cost of the closest path
	public int fCost = int.MaxValue;

    //Nodes have references to other nodes so it is possible to build a path.
    public Node parent = null;

    //Value of the node
    public Value value;

    //Constructor
    public Node(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;

        value = Value.FREE;
    }
}
public class AStar : MonoBehaviour
{

    //Constants
    private const int MAP_SIZE = 6;

    //Variables
    private List<string> map;
    private Node[,] nodeMap;

    // Start is called before the first frame update
    void Start()
    {
        map = new List<string>();
	    map.Add("------");
	    map.Add("XXX-XG");
	    map.Add("--X-XX");
        map.Add("--X-X-");
	    map.Add("--X-XX");
	    map.Add("-----S");

        //Parse the map
        nodeMap = new Node[MAP_SIZE, MAP_SIZE];
        Node start = null;
        Node goal = null;

        for (int y = 0; y < MAP_SIZE; y++)
        {
            for (int x = 0; x < MAP_SIZE; x++)
            {
                Node node = new Node(x, y);

                char currentChar = map[y][x];
                if (currentChar == 'X')
                {
                    node.value = Node.Value.BLOCKED;
                }
                else if (currentChar == 'G')
                {
                    goal = node;
                }
                else if (currentChar == 'S')
                {
                    start = node;
                }

                nodeMap[x, y] = node;

            }
        }

        //Execute AStar algorithm
        List<Node> nodePath = ExecuteAStar(start, goal);

	    //Burn path in the map
	    foreach(Node node in nodePath){
	    	char[] charArray = map[node.posY].ToCharArray();
	    	charArray[node.posX] = '@';
	    	map[node.posY] = new string (charArray);
	    	
	    }

        //print the map
        string mapString = "";
        foreach (string mapRow in map)
        {
            mapString += mapRow + '\n';
        }

        Debug.Log(mapString);

    }

    private List<Node> ExecuteAStar(Node start, Node goal)
    {
        //This list holds potential best path nodes that should be visited. It always starts with the origin.
        List<Node> openList = new List<Node>() { start };

        //This list keeps track of all nodes that have been visited.
        List<Node> closedList = new List<Node>();

        //Initialize the start node
        //f = g + h
        start.gCost = 0;
        start.fCost = CalculateHValue(start, goal);

        //Main algorithm
        while (openList.Count > 0)
        {
            //First, get node with lowest estimate cost to reach target
            Node current = openList[0];
            foreach (Node node in openList)
            {
                if (node.fCost < current.fCost)
                {
                    current = node;
                }
            }
            //Check if target has been reached
            if (current == goal)
            {
                return BuildPath(goal);
            }

            //Make sure current node will not be visited again.
            openList.Remove(current);
            closedList.Add(current);

            //Execute the algorithm in the current node's neighbors.
            List<Node> neighbors = GetNeighborNodes(current);
            foreach (Node neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    //If neighbor has already been visited, ignore.
                    continue;
                }

                if (!openList.Contains(neighbor))
                {
                    //If neighbor hasn't been scheduled for visiting, add it.
                    openList.Add(neighbor);
                }

                //Calculate new g value and verify if this value 
                //is better than what is stored in the neighbor.
                int candidateG = current.gCost + 1;
                if (candidateG >= neighbor.gCost)
                {
                    //If g value is greater or equal, 
                    //this does not belong to a good path(there was a better path calculated)
                    continue;
                }
                else
                {
                    //Otherwise we found a better way to reach this neighbor
                    //Initialize its values.
                    neighbor.parent = current;
                    neighbor.gCost = candidateG;
                    neighbor.fCost = neighbor.gCost + CalculateHValue(neighbor, goal);
                }


            }
        }

        //If reached, it means there are no more nodes to search. The algorithm failed.
        return new List<Node>();
    }

	//Once all nodes are populated, just read every parent and build the path.
    private List<Node> BuildPath(Node node)
    {
        List<Node> path = new List<Node>() { node };

        while (node.parent != null)
        {
            node = node.parent;
            path.Add(node);
        }

        return path;
    }

    private List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighbors = new List<Node>();

        //Verify all possible neighbors. Since we can only mov
        //horizontally and vertically, check these 4 possibilities.
        // Note that if node is blocked, it cannot be visited.

        //Left
        if (node.posX - 1 >= 0)
        {
            Node candidate = nodeMap[node.posX - 1, node.posY];
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }
        //Right
        if (node.posX + 1 <= MAP_SIZE - 1)
        {
            Node candidate = nodeMap[node.posX + 1, node.posY];
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }
        //Down
        if (node.posY - 1 >= 0)
        {
            Node candidate = nodeMap[node.posX, node.posY - 1];
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }
        //Up
        if (node.posY + 1 <= MAP_SIZE - 1)
        {
            Node candidate = nodeMap[node.posX, node.posY + 1];
            if (candidate.value != Node.Value.BLOCKED)
            {
                neighbors.Add(candidate);
            }
        }

        return neighbors;
    }

    //A simple estimate of the distance
    //Uses the manhattan distance.
    private int CalculateHValue(Node node1, Node node2)
    {
        return Mathf.Abs(node1.posX - node2.posX) + Mathf.Abs(node1.posY - node2.posY);
    }
}
