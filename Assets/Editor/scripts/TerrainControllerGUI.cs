using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainController))]
public class TerrainControllerGUI : Editor
{
       
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TerrainController terrainController = (TerrainController)target;

        if (GUILayout.Button("Refresh Terrain"))
        {
            terrainController.RefreshTerrain();
        }

        if (GUILayout.Button("Rotate Texture By 90"))
        {
            terrainController.RotateTerrainTextureBy90();
        }
    }
}

