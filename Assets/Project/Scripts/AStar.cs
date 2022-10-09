using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that holds information about certain position
//, so it's used in a pathfinding algorithm.
public class Node
{

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
    public NavTile value = null;

    //Constructor
    public Node(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
    }
}
public class AStar : MonoBehaviour
{
    //Public variables
    public GameObject backgroundContainer;
    public GameObject player;
    public GameObject enemy;

    //Variables
    private int mapWidth;
    private int mapHeight;
    private Node[,] nodeMap;

    public List<Node> FindPath()
    {
        //Preload values.
        mapHeight = backgroundContainer.transform.childCount;
        mapWidth = backgroundContainer.transform.GetChild(0).childCount;

        //Parse the map
        nodeMap = new Node[mapWidth, mapHeight];
        Node start = null;
        Node goal = null;

        for (int y = 0; y < backgroundContainer.transform.childCount; y++)
        {
            Transform backgroundRow = backgroundContainer.transform.GetChild(y);

            for (int x = 0; x < backgroundRow.transform.childCount; x++)
            {
                NavTile tile = backgroundRow.GetChild(x).GetComponent<NavTile>();

                Node node = new Node(x, y);
                node.value = tile;
                nodeMap[x, y] = node;
            }
        }

        start = FindNode(player);
        goal = FindNode(enemy);

        //Execute AStar algorithm
        List<Node> nodePath = ExecuteAStar(start, goal);
        nodePath.Reverse();
        return nodePath;
    }

    private Node FindNode(GameObject obj)
    {
        Collider2D[] collidingObjects = Physics2D.OverlapCircleAll(obj.transform.position, 0.2f);
        foreach (Collider2D collidingObject in collidingObjects)
        {
            if (collidingObject.gameObject.GetComponent<NavTile>() != null)
            {
                // This is the tile the object is on
                NavTile tile = collidingObject.gameObject.GetComponent<NavTile>();

                //Find the node which contains the tile
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Node node = nodeMap[x, y];
                        if (node.value == tile)
                        {
                            return node;
                        }
                    }
                }
            }
        }

        return null;
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
            neighbors.Add(candidate);

        }
        //Right
        if (node.posX + 1 <= mapWidth - 1)
        {
            Node candidate = nodeMap[node.posX + 1, node.posY];
            neighbors.Add(candidate);

        }
        //Down
        if (node.posY - 1 >= 0)
        {
            Node candidate = nodeMap[node.posX, node.posY - 1];
            neighbors.Add(candidate);
        }
        //Up
        if (node.posY + 1 <= mapHeight - 1)
        {
            Node candidate = nodeMap[node.posX, node.posY + 1];
            neighbors.Add(candidate);
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
