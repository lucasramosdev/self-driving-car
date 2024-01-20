using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wavefront : MonoBehaviour
{
    Node[,] grid;
    Vector2 gridWorldSize;
    NodeTuple goal;

    public void Initialise(Node[,] grid, Vector2 gridWorldSize)
    {
        this.grid = grid;
        this.gridWorldSize = gridWorldSize;
    }

    public void SetGoalNode(int x, int y, int value)
    {
        goal = new NodeTuple(x, y, value);
    }

    public void UpdateNodeValues()
    {
        List<NodeTuple> currentNodeList = new List<NodeTuple>{goal};


        while(currentNodeList.Count > 0)
        {
            List<NodeTuple> neighbours = new List<NodeTuple>();
            foreach(NodeTuple currentNode in currentNodeList)
            {
                grid[currentNode.x, currentNode.y].value = currentNode.value;

                CheckEastern(currentNode, neighbours);
                CheckWestern(currentNode, neighbours);
                CheckNorthern(currentNode, neighbours);
                CheckSouthern(currentNode, neighbours);

            }

            currentNodeList.Clear();
            foreach(NodeTuple neighbour in neighbours)
            {
                currentNodeList.Add(neighbour);
            }

        }
    }

    private void CheckEastern(NodeTuple currentNode, List<NodeTuple> neighbours)
    {
        Node nodeNeighbour = grid[currentNode.x+1, currentNode.y];
        if(currentNode.x < (gridWorldSize.x - 1) && nodeNeighbour.walkable == true && nodeNeighbour.value == 0)
        {
            if(nodeNeighbour.goal)
            {
                NodeTuple nextNeighbour = new NodeTuple(currentNode.x+1, currentNode.y, 0);
                CheckEastern(nextNeighbour, neighbours);
                return;
            }

            bool isPresent = false;
            int neighbourValue = currentNode.value + 1;
            foreach(NodeTuple neighbour_ in neighbours)
            {
                isPresent = neighbour_.x == currentNode.x+1 && neighbour_.y == currentNode.y && neighbour_.value == neighbourValue;
                if(isPresent) break;
            }
            NodeTuple neighbour = new NodeTuple(currentNode.x+1, currentNode.y, neighbourValue);
            if(!isPresent) neighbours.Add(neighbour);
        }
    }

    private void CheckWestern(NodeTuple currentNode, List<NodeTuple> neighbours)
    {
        
        Node nodeNeighbour = grid[currentNode.x-1, currentNode.y];
        if(currentNode.x >= 1 && nodeNeighbour.walkable == true && nodeNeighbour.value == 0)
        {
            if(nodeNeighbour.goal)
            {
                NodeTuple nextNeighbour = new NodeTuple(currentNode.x-1, currentNode.y, 0);
                CheckWestern(nextNeighbour, neighbours);
                return;
            }
            bool isPresent = false;
            int neighbourValue = currentNode.value + 1;
            foreach(NodeTuple neighbour_ in neighbours)
            {
                isPresent = neighbour_.x == currentNode.x-1 && neighbour_.y == currentNode.y && neighbour_.value == neighbourValue;
                if(isPresent) break;
            }
            NodeTuple neighbour = new NodeTuple(currentNode.x-1, currentNode.y, neighbourValue);
            if(!isPresent) neighbours.Add(neighbour);
        }
    }

    private void CheckNorthern(NodeTuple currentNode, List<NodeTuple> neighbours)
    {
        Node nodeNeighbour = grid[currentNode.x, currentNode.y-1];
        if(currentNode.y >= 1 && nodeNeighbour.walkable == true && nodeNeighbour.value == 0)
        {
            if(nodeNeighbour.goal)
            {
                NodeTuple nextNeighbour = new NodeTuple(currentNode.x, currentNode.y-1, 0);
                CheckNorthern(nextNeighbour, neighbours);
                return;
            }
            bool isPresent = false;
            int neighbourValue = currentNode.value + 1;
            foreach(NodeTuple neighbour_ in neighbours)
            {
                isPresent = neighbour_.x == currentNode.x && neighbour_.y == currentNode.y-1 && neighbour_.value == neighbourValue;
                if(isPresent) break;
            }
            NodeTuple neighbour = new NodeTuple(currentNode.x, currentNode.y-1, neighbourValue);
            if(!isPresent) neighbours.Add(neighbour);
        }
    }

    private void CheckSouthern(NodeTuple currentNode, List<NodeTuple> neighbours)
    {
        Node nodeNeighbour = grid[currentNode.x, currentNode.y+1];
        if(currentNode.y < (gridWorldSize.y - 1) && nodeNeighbour.walkable == true && nodeNeighbour.value == 0)
        {
            if(nodeNeighbour.goal)
            {
                NodeTuple nextNeighbour = new NodeTuple(currentNode.x, currentNode.y+1, 0);
                CheckSouthern(nextNeighbour, neighbours);
                return;
            }
            bool isPresent = false;
            int neighbourValue = currentNode.value + 1;
            foreach(NodeTuple neighbour_ in neighbours)
            {
                isPresent = neighbour_.x == currentNode.x && neighbour_.y == currentNode.y+1 && neighbour_.value == neighbourValue;
                if(isPresent) break;
            }
            NodeTuple neighbour = new NodeTuple(currentNode.x, currentNode.y+1, neighbourValue);
            if(!isPresent) neighbours.Add(neighbour);
        }
    }
}


public class NodeTuple
{
    public int value;
    public int x;
    public int y;
    public NodeTuple(int _x, int _y, int _value)
    {
        value = _value;
        x = _x;
        y = _y;
    }
}
