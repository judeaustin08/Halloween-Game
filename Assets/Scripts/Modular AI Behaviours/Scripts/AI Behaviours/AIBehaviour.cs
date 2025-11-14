using Pathfinding;
using UnityEngine;

// Set up to work with the Unity AstarPathfindingProject by Aron Granberg
public abstract class AIBehaviour : ScriptableObject
{
    protected GameObject parent;
    public float minimumInterval;
    public float randomInterval;
    public bool continuous = false;
    public float speedModifier = 0;
    public float multiplicativeSpeedMultiplier = 1;
    public void Initialize(GameObject parent)
    {
        this.parent = parent;
    }
    public abstract Vector3 SelectTarget();
    
    public void Gizmos()
    {
        // Can be overwritten in child classes
    }
}
