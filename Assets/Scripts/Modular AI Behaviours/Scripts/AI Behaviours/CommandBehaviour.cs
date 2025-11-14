using UnityEngine;

[CreateAssetMenu(fileName = "Command Behaviour", menuName = "AI Behaviours/Behaviours/New Command Behaviour")]
public class CommandBehaviour : AIBehaviour
{
    public string targetPropertyName = "_Target";

    public new void Initialize(GameObject parent)
    {
        base.Initialize(parent);
    }

    public override Vector3 SelectTarget()
    {
        return ((Transform)typeof(NPC).GetProperty(targetPropertyName).GetValue(parent.GetComponent<NPC>())).position;
    }
}