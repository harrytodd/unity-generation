using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Marching)), CanEditMultipleObjects]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Marching marching = (Marching)target;

        if(DrawDefaultInspector())
        {
            if(marching.autoUpdate)
            {
                marching.Generate();
            }
        }

        if(GUILayout.Button("Generate"))
        {
            marching.Generate();
        }
    }
}
