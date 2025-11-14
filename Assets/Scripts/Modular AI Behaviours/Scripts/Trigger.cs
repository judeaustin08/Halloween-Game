using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName="Trigger", menuName="AI Behaviours/New Trigger")]
public class Trigger : ScriptableObject
{
    private NPC parent;

    public enum Type
    {
        BOOLEAN,
        FLOAT,
        METHOD
    }
    public Type type;
    public string propertyName;

    public bool setBooleanValue;

    public float setFloatValue;

    public void Initialize(NPC parent)
    {
        this.parent = parent;
    }

    public void Invoke()
    {
        switch (type)
        {
            case Type.BOOLEAN:
                typeof(NPC).GetProperty(propertyName).SetValue(parent, setBooleanValue);
                break;
            case Type.FLOAT:
                typeof(NPC).GetProperty(propertyName).SetValue(parent, setFloatValue);
                break;
            case Type.METHOD:
                Debug.Log("Triggering");
                typeof(NPC).InvokeMember(propertyName, BindingFlags.Public, null, parent, null);
                break;
        }
    }
}