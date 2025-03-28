using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SectorEventSO))]
public class SectorEventSOEditor : Editor
{
    private GUIStyle boldStyle;
    private GUIStyle subHeaderStyle;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Initialize styles
        if (boldStyle == null)
        {
            boldStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };
        }

        if (subHeaderStyle == null)
        {
            subHeaderStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };
        }

        // Draw base class properties first
        DrawPropertiesExcluding(serializedObject, "eventEffects", "numberOfEventEffects", "crewmateToRecruit", "hasChoices", "numberOfChoices", "choices");

        // Event Effects Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects & Crewmates", EditorStyles.boldLabel);
        
        // Draw crewmate field
        SerializedProperty crewmateProp = serializedObject.FindProperty("crewmateToRecruit");
        EditorGUILayout.PropertyField(crewmateProp);
        
        SerializedProperty numEventEffectsProp = serializedObject.FindProperty("numberOfEventEffects");
        EditorGUILayout.PropertyField(numEventEffectsProp, new GUIContent("Number of Event Effects"));

        SerializedProperty eventEffectsProp = serializedObject.FindProperty("eventEffects");

        // Ensure event effects list matches numberOfEventEffects
        while (eventEffectsProp.arraySize < numEventEffectsProp.intValue)
        {
            eventEffectsProp.InsertArrayElementAtIndex(eventEffectsProp.arraySize);
        }

        while (eventEffectsProp.arraySize > numEventEffectsProp.intValue)
        {
            eventEffectsProp.DeleteArrayElementAtIndex(eventEffectsProp.arraySize - 1);
        }

        // Display event effects without boxes
        EditorGUI.indentLevel++;
        for (int i = 0; i < eventEffectsProp.arraySize; i++)
        {
            EditorGUILayout.PropertyField(eventEffectsProp.GetArrayElementAtIndex(i), new GUIContent($"Effect {i + 1}"));
        }
        EditorGUI.indentLevel--;

        // Choices Section
        EditorGUILayout.Space();
        SerializedProperty hasChoicesProp = serializedObject.FindProperty("hasChoices");
        EditorGUILayout.PropertyField(hasChoicesProp);

        if (hasChoicesProp.boolValue)
        {
            SerializedProperty numChoicesProp = serializedObject.FindProperty("numberOfChoices");
            EditorGUILayout.PropertyField(numChoicesProp);

            SerializedProperty choicesProp = serializedObject.FindProperty("choices");

            // Ensure choices list matches numberOfChoices
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
                SerializedProperty crewmateRewardProp = choiceProp.FindPropertyRelative("crewmate");
                SerializedProperty numChoiceEffectsProp = choiceProp.FindPropertyRelative("numberOfChoiceEffects");
                SerializedProperty choiceEffectsProp = choiceProp.FindPropertyRelative("choiceEffects");
                SerializedProperty eventsProp = choiceProp.FindPropertyRelative("possibleEvents");

                // Add spacing before each choice
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox); // Boxed section
                EditorGUILayout.LabelField($"Choice {i + 1}", boldStyle);

                EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Choice Description"));
                
                EditorGUILayout.PropertyField(crewmateRewardProp, new GUIContent("Crewmate Reward"));
                
                // Choice Effects Section
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Choice Effects", subHeaderStyle);
                EditorGUILayout.PropertyField(numChoiceEffectsProp, new GUIContent("Number of Effects"));

                // Ensure choice effects list matches numberOfChoiceEffects
                while (choiceEffectsProp.arraySize < numChoiceEffectsProp.intValue)
                {
                    choiceEffectsProp.InsertArrayElementAtIndex(choiceEffectsProp.arraySize);
                }

                while (choiceEffectsProp.arraySize > numChoiceEffectsProp.intValue)
                {
                    choiceEffectsProp.DeleteArrayElementAtIndex(choiceEffectsProp.arraySize - 1);
                }

                // Display choice effects
                EditorGUI.indentLevel++;
                for (int j = 0; j < choiceEffectsProp.arraySize; j++)
                {
                    EditorGUILayout.PropertyField(choiceEffectsProp.GetArrayElementAtIndex(j), new GUIContent($"Effect {j + 1}"));
                }
                EditorGUI.indentLevel--;

                // Display possible events section
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Possible Events", subHeaderStyle);

                for (int j = 0; j < eventsProp.arraySize; j++)
                {
                    SerializedProperty eventProp = eventsProp.GetArrayElementAtIndex(j);
                    SerializedProperty eventOptionProp = eventProp.FindPropertyRelative("eventOption");
                    SerializedProperty probabilityProp = eventProp.FindPropertyRelative("probability");

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.PropertyField(eventOptionProp, new GUIContent($"Event {j + 1}"));
                    EditorGUILayout.PropertyField(probabilityProp, new GUIContent($"Probability (%)"));
                    EditorGUILayout.EndVertical();
                }

                // Buttons to add/remove events
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+ Add Event Option"))
                {
                    eventsProp.InsertArrayElementAtIndex(eventsProp.arraySize);
                }

                if (eventsProp.arraySize > 0 && GUILayout.Button("- Remove Last Event"))
                {
                    eventsProp.DeleteArrayElementAtIndex(eventsProp.arraySize - 1);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
