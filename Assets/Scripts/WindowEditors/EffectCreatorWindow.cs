using UnityEditor;
using UnityEngine;
using System.IO;

public class EffectCreatorWindow : EditorWindow
{
    private string effectName = "New Effect"; 
    private EffectType selectedEffectType;
    private int amount = 0;
    private string effectDescription = "Enter effect description";

    [MenuItem("Tools/Effect Creator")]
    public static void ShowWindow()
    {
        GetWindow<EffectCreatorWindow>("Effect Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Effect", EditorStyles.boldLabel);

        effectName = EditorGUILayout.TextField("Effect Name", effectName);
        selectedEffectType = (EffectType)EditorGUILayout.EnumPopup("Effect Type", selectedEffectType);
        amount = EditorGUILayout.IntField("Effect Amount", amount);
        effectDescription = EditorGUILayout.TextField("Effect Description", effectDescription);

        if (GUILayout.Button("Create Effect")) CreateEffect();
    }

    private void CreateEffect()
    {
        string path = "Assets/ScriptableObjects/Effects";

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string fileName = $"{effectName}_{selectedEffectType}_Amount{amount}.asset";
        string fullPath = Path.Combine(path, fileName);

        if (File.Exists(fullPath))
        {
            Debug.LogWarning($"Effect \"{fileName}\" already exists! No new effect created.");
            return;
        }

        GenericEffect newEffect = ScriptableObject.CreateInstance<GenericEffect>();
        newEffect.effectName = effectName;
        newEffect.effectType = selectedEffectType;
        newEffect.amount = amount;
        newEffect.description = effectDescription;

        AssetDatabase.CreateAsset(newEffect, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Effect \"{fileName}\" created successfully!");
    }
}
