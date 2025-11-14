using UnityEngine;

[CreateAssetMenu(fileName = "Flee Behaviour", menuName = "AI Behaviours/Behaviours/New Flee Behaviour")]
public class FleeBehaviour : AIBehaviour
{
    public string targetPropertyName = "_Target";
    private Transform target;
    [SerializeField] private float pathDistance;

    public new void Initialize(GameObject parent)
    {
        base.Initialize(parent);
        target = (Transform)typeof(NPC).GetProperty(targetPropertyName).GetValue(parent.GetComponent<NPC>());
    }

    public override Vector3 SelectTarget()
    {
        Vector3 dir2T = (target.position - parent.transform.position).normalized;
        return parent.transform.position - dir2T * pathDistance;
    }
}