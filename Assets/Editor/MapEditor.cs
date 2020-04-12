using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();

		//if (GUI.changed)
		//{
		//	MapGenerator map = target as MapGenerator;
		//
		//	map.GenerateMap();
		//}	

		MapGenerator map = target as MapGenerator;

		if (DrawDefaultInspector())
		{
			map.GenerateMap();
		}

		if (GUILayout.Button("Generate Map"))
		{
			map.GenerateMap();
		}
	}
}
