using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SectorEventSO))]
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
                SerializedProperty crewmateRewardProp = choiceProp.FindPropertyRelative("crewmate");

                EditorGUILayout.PropertyField(descriptionProp, new GUIContent($"Choice {i + 1}"));
                EditorGUILayout.PropertyField(effectProp, new GUIContent($"Choice {i + 1} Effect"));
                EditorGUILayout.PropertyField(crewmateRewardProp, new GUIContent($"Choice {i + 1} Crew"));
                EditorGUILayout.Space();
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
