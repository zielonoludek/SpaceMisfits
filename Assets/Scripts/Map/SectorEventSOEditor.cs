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

                EditorGUILayout.PropertyField(descriptionProp, new GUIContent($"Choice {i + 1}"));
                EditorGUILayout.PropertyField(effectProp, new GUIContent($"Choice {i + 1} Effect"));
            }
        }
        serializedObject.ApplyModifiedProperties();
        
        // DrawPropertiesExcluding(serializedObject, "hasChoices", "choice1Description", "choice1Effect", "choice2Description", "choice2Effect");
        //
        // SerializedProperty hasChoicesProp = serializedObject.FindProperty("hasChoices");
        // EditorGUILayout.PropertyField(hasChoicesProp);
        //
        // if (hasChoicesProp.boolValue)
        // {
        //     SerializedProperty choice1Prop = serializedObject.FindProperty("choice1Description");
        //     SerializedProperty choice1EffectProp = serializedObject.FindProperty("choice1Effect");
        //     SerializedProperty choice2Prop = serializedObject.FindProperty("choice2Description");
        //     SerializedProperty choice2EffectProp = serializedObject.FindProperty("choice2Effect");
        //
        //     EditorGUILayout.PropertyField(choice1Prop, new GUIContent("Choice 1"));
        //     EditorGUILayout.PropertyField(choice1EffectProp, new GUIContent("Choice 1 Effect"));
        //     EditorGUILayout.PropertyField(choice2Prop, new GUIContent("Choice 2"));
        //     EditorGUILayout.PropertyField(choice2EffectProp, new GUIContent("Choice 2 Effect"));
        // }
        //
        // serializedObject.ApplyModifiedProperties();
    }
}
