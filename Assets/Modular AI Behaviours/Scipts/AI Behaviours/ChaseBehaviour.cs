using UnityEngine;

[CreateAssetMenu(fileName="Chase Behaviour", menuName="AI Behaviours/Behaviours/New Chase Behaviour")]
public class ChaseBehaviour : AIBehaviour
{
    public string targetPropertyName = "_Target";
    public string sightPropertyName = "_SeeingTarget";

    private Vector3 lastKnownPosition;
    private Transform target;

    public override void Initialize(GameObject parent)
    {
        this.parent = parent;
        target = (Transform)typeof(NPC).GetProperty(targetPropertyName).GetValue(parent.GetComponent<NPC>());
    }

    /*
    Returns the last known position of the target. The last known position is updated if:
     - The NPC can see the target
     - The target is within a very close distance of the target (NPC can hear)
    */
    public override Vector3 SelectTarget()
    {
        Vector3 t_pos = new(
            target.position.x,
            parent.transform.position.y,
            target.position.z
        );

        // If the NPC can see the player, using the sight property from the NPC class
        if ((bool)typeof(NPC).GetProperty(sightPropertyName).GetValue(parent.GetComponent<NPC>()))
            lastKnownPosition = t_pos;

        return lastKnownPosition;
    }
}