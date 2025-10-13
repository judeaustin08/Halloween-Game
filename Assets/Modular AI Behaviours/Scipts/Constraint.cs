using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Constraint", menuName = "AI Behaviours/New Constraint")]
public class Constraint : ScriptableObject
{
    public enum Type
    {
        DISTANCE,
        FLOAT,
        BOOLEAN
    }
    public enum ComparisonType
    {
        EQUAL,
        NOT_EQUAL,
        GREATER_THAN,
        LESS_THAN
    }
    public enum BooleanEvaluationType
    {
        TRUE,
        FALSE
    }

    public Type type;

    public GameObject parent;

    [Tooltip("The type of comparison")]
    public ComparisonType comparisonType;
    public BooleanEvaluationType boolEvaluationType;

    public string propertyName;

    [Tooltip("Value with which to compare the provided property")]
    public float compareValue;

    public static bool Evaluate(IEnumerable<Constraint> list)
    {
        foreach (Constraint c in list)
            if (!c.Evaluate())
                return false;

        return true;
    }

    public void Initialize(GameObject parent)
    {
        this.parent = parent;
    }

    public bool Evaluate()
    {
        // Get property from parent
        System.Reflection.PropertyInfo property = typeof(NPC).GetProperty(propertyName);;
        NPC parentNPC = parent.GetComponent<NPC>();

        switch (type)
        {
            case Type.DISTANCE:
                Transform target = (Transform)property.GetValue(parentNPC);
                float distance = Vector3.Distance(
                    parent.transform.position,
                    target.transform.position
                );
                return Compare(distance, compareValue);
            case Type.FLOAT:
                return Compare((float)property.GetValue(parentNPC), compareValue);
            case Type.BOOLEAN:
                bool result = (bool)property.GetValue(parentNPC);
                return result == (boolEvaluationType == BooleanEvaluationType.TRUE);
            default:
                return false;
        }
    }

    // Custom method to compare two IComparable objects based on the ComparisonType
    public bool Compare<T>(T a, T b) where T : IComparable<T>
    {
        switch (comparisonType)
        {
            case ComparisonType.EQUAL:
                return a.CompareTo(b) == 0;
            case ComparisonType.NOT_EQUAL:
                return a.CompareTo(b) != 0;
            case ComparisonType.GREATER_THAN:
                return a.CompareTo(b) > 0;
            case ComparisonType.LESS_THAN:
                return a.CompareTo(b) < 0;
            default:
                return false;
        }
    }
}