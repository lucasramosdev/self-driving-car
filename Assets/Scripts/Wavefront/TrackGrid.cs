using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackGrid : MonoBehaviour
{
    [SerializeField] Wavefront waveFront;
    [SerializeField] Transform goal;
    public LayerMask unwalkableMask;
    public LayerMask goalMask;
    public LayerMask viewerMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;
    public bool drawGrid = false;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        PropageteWavefront();

    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 world_bottom_left = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 world_point = world_bottom_left + Vector3.right * (x * nodeDiameter + nodeRadius)+ Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(world_point, nodeRadius, unwalkableMask);
                bool goal = Physics.CheckSphere(world_point, nodeRadius, goalMask);
                int value = walkable ? 0 : -1;
                grid[x, y] = new Node(walkable, world_point, x, y, value, goal);
            }   
        }
    }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; 
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; 

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

        return grid[x,y];
    }

    public Node GetGoalNode(Vector3 position)
    {
        Node node = NodeFromWorldPoint(position);
        if(node.value == -1){
        List<Node> neighbours = new List<Node>();
        CheckNode(node.gridX, node.gridY-1, neighbours);
        CheckNode(node.gridX, node.gridY+1, neighbours);
        CheckNode(node.gridX-1, node.gridY, neighbours);
        CheckNode(node.gridX+1, node.gridY, neighbours);
        return neighbours[0];
        }
        return node;
    }

    void CheckNode(int x, int y, List<Node> nodes)
    {
        Node node = grid[x, y];
        if(node.value != -1) nodes.Add(node); 
    }

    void PropageteWavefront()
    {
        waveFront.Initialise(grid, gridWorldSize);
        Node goalNode = NodeFromWorldPoint(goal.position);
        waveFront.SetGoalNode(goalNode.gridX, goalNode.gridY, goalNode.value);
        waveFront.UpdateNodeValues();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid != null && drawGrid)
        {
            foreach(Node node in grid)
            {
                if(Physics.CheckSphere(node.worldPosition, nodeRadius, viewerMask))
                {
                    Handles.Label(node.worldPosition, node.value.ToString());
                }
            }
        }
    }
}
