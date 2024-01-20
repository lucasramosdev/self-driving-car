using UnityEngine;

public class Node
{
    public bool walkable;
    public bool goal;
    public Vector3 worldPosition;
    public int value = 0;
    public int gridX;
    public int gridY;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _value, bool _goal)
    {
        walkable = _walkable;
        goal = _goal;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
        value = _value;
    }
}
