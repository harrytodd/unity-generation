using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Create a button for us to press that updates the map without having to be in play mode
[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
                mapGen.GenerateMap();
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
