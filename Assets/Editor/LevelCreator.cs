/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "tool_level_creator", menuName = "FFEditor/Tool/Level Creator" ) ]
public class LevelCreator : ScriptableObject
{
  [ Title( "Boss Setup" ) ]
    [ SerializeField ] public int boss_power;

  [ Title( "Environment Setup" ) ]
    [ SerializeField ] public int ground_count;
    [ SerializeField ] public float boss_offset;

  [ Title( "Data Setup" ) ]
	[ SerializeField ] GameObject boss_object;
    [ SerializeField ] GroundData data_ground;
    [ SerializeField ] FinishLineData data_finishLine;
    [ SerializeField ] FinalStageData data_finalStage;

    [ Button() ]
    public void CreateEnvironment()
    {
		var environmentParent = GameObject.Find( "environment" ).transform;
   
		// EditorUtility.SetDirty( environmentParent );
		EditorSceneManager.MarkAllScenesDirty();
		environmentParent.DestoryAllChildren();

		int i;
		for( i = 0; i < ground_count; i++ )
        {
			var ground = PrefabUtility.InstantiatePrefab( data_ground.ground_object ) as GameObject;
			ground.name = "ground_" + ( i + 1 );
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

		GameObject.DestroyImmediate( GameObject.Find( "boss" ) ); // Destory Boss

		var boss = PrefabUtility.InstantiatePrefab( boss_object ) as GameObject;
		boss.name = "boss";
		boss.transform.SetParent( null );
		boss.transform.SetSiblingIndex( GameObject.Find( "player" ).transform.GetSiblingIndex() );
		boss.transform.position = finalStage.transform.position;

		boss.GetComponentInChildren< Enemy >().SetEnemy( boss_power, false, true );

		AssetDatabase.SaveAssets();
	}

	[ Button() ]
	public void LineUpEnemies( int count, float offset )
	{
		EditorSceneManager.MarkAllScenesDirty();

		var objects = SceneManager.GetActiveScene().GetRootGameObjects();

		var selection = Selection.activeGameObject;

		var startIndex = selection.transform.GetSiblingIndex();
		var startPosition = selection.transform.position.z;

		for( var i = 1; i <= count; i++ )
		{
			var sibling = objects[ startIndex + i ].transform;
			sibling.position = sibling.position.SetZ( startPosition + i * offset );
		}

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