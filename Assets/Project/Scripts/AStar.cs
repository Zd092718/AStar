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
    public int gCost = 0;
    //hCost is a heuristic that estimates the cost of the closest path
    public int fCost = 0;

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
        map.Add("G-----");
        map.Add("XXXXX-");
        map.Add("S-X-X-");
        map.Add("--X-X-");
        map.Add("--X-X-");
        map.Add("------");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
