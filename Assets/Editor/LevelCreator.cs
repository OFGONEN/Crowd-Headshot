/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "tool_level_creator", menuName = "FFEditor/Tool/Level Creator" ) ]
public class LevelCreator : ScriptableObject
{

  [ Title( "Environment Setup" ) ]
    [ SerializeField ] public int ground_count;
    [ SerializeField ] public float boss_offset;
    [ SerializeField ] public float boss_power;

  [ Title( "Data Setup" ) ]
    [ SerializeField ] GroundData data_ground;
    [ SerializeField ] FinishLineData data_finishLine;
    [ SerializeField ] FinalStageData data_finalStage;

    [ Button() ]
    public void CreateEnvironment()
    {
		var environmentParent = GameObject.Find( "environment" ).transform;
   
		EditorUtility.SetDirty( environmentParent );
		environmentParent.DestoryAllChildren();

		int i;
		for( i = 0; i < ground_count; i++ )
        {
			var ground = PrefabUtility.InstantiatePrefab( data_ground.ground_object ) as GameObject;
			ground.transform.SetParent( environmentParent );
			ground.transform.localPosition = Vector3.forward * i * data_ground.ground_length;
		}

		i -= 1;

		var finishLine = PrefabUtility.InstantiatePrefab( data_finishLine.finishLine_object ) as GameObject;
		finishLine.transform.SetParent( environmentParent );
        finishLine.transform.localPosition = Vector3.forward * ( i * data_ground.ground_length + data_finishLine.finishLine_offset );

		var finalStage = PrefabUtility.InstantiatePrefab( data_finalStage.finalStage_object ) as GameObject;
		finalStage.transform.SetParent( environmentParent );
		finalStage.transform.localPosition = Vector3.forward * ( i * data_ground.ground_length + data_finalStage.finalStage_offset + boss_offset );

		AssetDatabase.SaveAssets();
	}
}

[ Serializable ]
public struct GroundData
{
	public GameObject ground_object;
    public float ground_length;
}

[ Serializable ]
public struct FinishLineData
{
	public GameObject finishLine_object;
    public float finishLine_offset;
}

[ Serializable ]
public struct FinalStageData
{
	public GameObject finalStage_object;
    public float finalStage_offset;
}