using System.Linq;
using PathCreation;
using UnityEngine;

public class DeathLaser : MonoBehaviour
{
    
    public bool canMove = false;
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    
    void Start()
    {
        if (pathCreator != null)
        {
            pathCreator.pathUpdated += OnPathChanged;
        }
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if(canMove)
        {
            MoveLaser();
        }
    }

    public void Reset()
    {
        canMove = false;
        distanceTravelled = 0;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
    }

    private void MoveLaser()
    {
        if (pathCreator != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
    }

    public void AllowMoviment()
    {
        if(!canMove) canMove = true;
    }

    void OnPathChanged() 
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}
