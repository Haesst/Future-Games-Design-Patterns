using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    int chosenInt = 0;
    int numberOfMaps = 0;
    string[] category;
    string[] excludedVariables = new string[] { "mapCount" };

    public override void OnInspectorGUI()
    {
        int newNumberOfMaps = serializedObject.FindProperty("mapCount").intValue;

        if (newNumberOfMaps != numberOfMaps)
        {
            numberOfMaps = newNumberOfMaps;
            category = new string[numberOfMaps + 1];

            category[0] = "Select map";

            for (int i = 0; i < numberOfMaps; i++)
            {
                category[i + 1] = $"Map {i + 1}";
            }
        }

        if (numberOfMaps > 0)
        {
            MapGenerator mapGenerator = target as MapGenerator;

            EditorGUI.BeginChangeCheck();
            chosenInt = EditorGUILayout.Popup("Choose map", chosenInt, category, EditorStyles.popup);

            if(EditorGUI.EndChangeCheck())
            {
                mapGenerator.GenerateMap(chosenInt - 1);
            }
        }

        DrawPropertiesExcluding(serializedObject, excludedVariables);
        serializedObject.ApplyModifiedProperties();
    }
}
