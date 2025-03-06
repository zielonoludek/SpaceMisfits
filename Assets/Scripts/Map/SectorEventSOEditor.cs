using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventSO))]
public class SectorEventSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, "hasChoices", "numberOfChoices", "choices");
        
        SerializedProperty hasChoicesProp = serializedObject.FindProperty("hasChoices");
        EditorGUILayout.PropertyField(hasChoicesProp);

        if (hasChoicesProp.boolValue)
        {
            SerializedProperty numChoicesProp = serializedObject.FindProperty("numberOfChoices");
            EditorGUILayout.PropertyField(numChoicesProp);
            
            SerializedProperty choicesProp = serializedObject.FindProperty("choices");
            
            while (choicesProp.arraySize < numChoicesProp.intValue)
            {
                choicesProp.InsertArrayElementAtIndex(choicesProp.arraySize);
            }
            while (choicesProp.arraySize > numChoicesProp.intValue)
            {
                choicesProp.DeleteArrayElementAtIndex(choicesProp.arraySize - 1);
            }
            
            for (int i = 0; i < choicesProp.arraySize; i++)
            {
                SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(i);
                SerializedProperty descriptionProp = choiceProp.FindPropertyRelative("choiceDescription");
                SerializedProperty effectProp = choiceProp.FindPropertyRelative("choiceEffect");

                EditorGUILayout.PropertyField(descriptionProp, new GUIContent($"Choice {i + 1}"));
                EditorGUILayout.PropertyField(effectProp, new GUIContent($"Choice {i + 1} Effect"));
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
