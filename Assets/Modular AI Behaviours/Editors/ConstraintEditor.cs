using static Constraint;

using UnityEditor;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(Constraint))]
public class ConstraintEditor : Editor
{
    SerializedProperty type_prop;

    SerializedProperty comparisonType_prop;
    SerializedProperty boolEvaluationType_prop;

    SerializedProperty propertyName_prop;
    SerializedProperty compareValue_prop;

    private void OnEnable()
    {
        type_prop = serializedObject.FindProperty("type");
        comparisonType_prop = serializedObject.FindProperty("comparisonType");
        boolEvaluationType_prop = serializedObject.FindProperty("boolEvaluationType");

        propertyName_prop = serializedObject.FindProperty("propertyName");
        compareValue_prop = serializedObject.FindProperty("compareValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PropertyField(type_prop);
        Type type = (Type)type_prop.enumValueIndex;

        switch (type)
        {
            case Type.DISTANCE:
                PropertyField(propertyName_prop);
                PropertyField(compareValue_prop);
                PropertyField(comparisonType_prop);
                break;
            case Type.FLOAT:
                PropertyField(propertyName_prop);
                PropertyField(comparisonType_prop);
                PropertyField(compareValue_prop);
                break;
            case Type.BOOLEAN:
                PropertyField(propertyName_prop);
                PropertyField(boolEvaluationType_prop);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}