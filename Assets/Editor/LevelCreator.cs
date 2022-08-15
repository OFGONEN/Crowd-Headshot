/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "tool_level_creator", menuName = "FFEditor/Tool/Level Creator" ) ]
public class LevelCreator : ScriptableObject
{

  [ Title( "Environment Setup" ) ]
    [ SerializeField ] public int ground_count;

  [ Title( "Data Setup" ) ]
    [ SerializeField ] GroundData data_ground;

    [ Button() ]
    public void CreateEnvironment()
    {
		var environmentParent = GameObject.Find( "environment" ).transform;

		EditorUtility.SetDirty( environmentParent );
		environmentParent.DestoryAllChildren();

        for( var i = 0; i < ground_count; i++ )
        {
			var ground = PrefabUtility.InstantiatePrefab( data_ground.ground_object ) as GameObject;
			ground.transform.SetParent( environmentParent );
			ground.transform.localPosition = Vector3.forward * i * data_ground.ground_length;
		}

		AssetDatabase.SaveAssets();
	}
}

[ System.Serializable ]
public struct GroundData
{
	public GameObject ground_object;
    public float ground_length;
}