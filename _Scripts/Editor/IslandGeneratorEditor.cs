using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(IslandGenerator))]
public class IslandGeneratorEditor : Editor
{
    private bool showTilemaps;
    private bool showNoiseConfig;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        IslandGenerator generator = (IslandGenerator)target;

        EditorToolbox.Toggle(generator.DEBUG, "Debug? ", 100, EditorStyles.boldLabel, out generator.DEBUG);

        if (GUILayout.Button("ReGenerate Island"))
        {
            generator.StartCoroutine(generator.GenerateIsland());
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("biomes"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("landTile"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("waterTile"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("coastTile"));

        EditorToolbox.ValueToggle(showTilemaps, "Show Tilemaps? ", out showTilemaps, new List<SerializedProperty> {
            serializedObject.FindProperty("oceanTilemap"),
            serializedObject.FindProperty("shallowTilemap"),
            serializedObject.FindProperty("riverStartPointTilemap"),
            serializedObject.FindProperty("coastTilemap"),
            serializedObject.FindProperty("landTilemap"),
            serializedObject.FindProperty("treesTilemap"),
            serializedObject.FindProperty("structureTilemap")
        });
        EditorToolbox.ValueToggle(showNoiseConfig, "Show Noise Config? ", out showNoiseConfig, new List<SerializedProperty> {
            serializedObject.FindProperty("seed"),
            serializedObject.FindProperty("biomeNoiseMap"),
            serializedObject.FindProperty("noiseConfig"),
            serializedObject.FindProperty("coastPeakHeight"),
            serializedObject.FindProperty("oceanPeakHeight")
        });

        //Options
        EditorToolbox.ValueToggle(generator.generateTrees, "Generate Trees? ", out generator.generateTrees, new List<SerializedProperty> {
            serializedObject.FindProperty("treesNoise"),
            serializedObject.FindProperty("treeTypes")
        });
        EditorToolbox.ValueToggle(generator.generateRiver, "Generate Rivers? ", out generator.generateRiver, new List<SerializedProperty> {
            serializedObject.FindProperty("riverTile"),
            serializedObject.FindProperty("riverStartPointTile"),
            serializedObject.FindProperty("riverCount"),
            serializedObject.FindProperty("riverLengthMax")
        });
        EditorToolbox.ValueToggle(generator.generateVillages, "Generate Villages? ", out generator.generateVillages, new List<SerializedProperty> {
            serializedObject.FindProperty("villagesCount"),
            serializedObject.FindProperty("villageSize"),
            serializedObject.FindProperty("villageEmptyHolderTile"),
            serializedObject.FindProperty("villageTile")
        });
        EditorToolbox.ValueToggle(generator.generateCaves, "Generate Caves? ", out generator.generateCaves, new List<SerializedProperty> {
            serializedObject.FindProperty("cavesCount"),
            serializedObject.FindProperty("caveTile"),
            serializedObject.FindProperty("minDistFromCentre"),
            serializedObject.FindProperty("maxDistFromCentre")
        });
        EditorToolbox.ValueToggle(generator.generateFountain, "Generate Fountain? ", out generator.generateFountain, new List<SerializedProperty> {
            serializedObject.FindProperty("structures")
        });



        serializedObject.ApplyModifiedProperties();
    }

}
public static class EditorToolbox
{
    public static bool ValueToggle(bool value, string label, out bool result,List<SerializedProperty> properties = null)
    {
        EditorGUILayout.Space(3);
        Toggle(value, label, 160, EditorStyles.boldLabel, out value);
        result = value;
        if (value)
        {
            if (properties != null)
            {
                foreach (SerializedProperty property in properties)
                {
                    EditorGUILayout.PropertyField(property,true);
                }
            }
        }

        return value;
    }
    public static bool ValueToggle(bool value, out bool result)
    {
        ValueToggle(value, "", out result);
        return value;
    }
    public static bool Toggle(bool value, string label, int width, GUIStyle style, out bool result)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(label, style, GUILayout.Width(width));
        value = GUILayout.Toggle(value, GUIContent.none);
        EditorGUILayout.EndHorizontal();
        result = value;
        return value;
    }
}