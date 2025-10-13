using static Trigger;

using UnityEditor;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(Trigger))]
public class TriggerEditor : Editor
{
    SerializedProperty type_prop;
    SerializedProperty propertyName_prop;

    SerializedProperty setBooleanValue_prop;

    SerializedProperty setFloatValue_prop;

    private void OnEnable()
    {
        type_prop = serializedObject.FindProperty("type");
        propertyName_prop = serializedObject.FindProperty("propertyName");

        setBooleanValue_prop = serializedObject.FindProperty("setBooleanValue");

        setFloatValue_prop = serializedObject.FindProperty("setFloatValue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PropertyField(type_prop);
        Type type = (Type)type_prop.enumValueIndex;

        PropertyField(propertyName_prop);

        switch (type)
        {
            case Type.BOOLEAN:
                PropertyField(setBooleanValue_prop);
                break;
            case Type.FLOAT:
                PropertyField(setFloatValue_prop);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}