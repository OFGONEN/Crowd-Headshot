/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using DG.Tweening;

public class WeaponLoot : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
	[ SerializeField ] IntGameEvent event_gun_change;
	[ SerializeField ] SharedReferenceNotifier notif_player_transform;

  [ Title( "Components" ) ]
    [ SerializeField ] MeshFilter loot_mesh;
	[ SerializeField ] Respond respond_scope_off;

	int gun_index;
	float loot_travel_progression;
	Vector3 player_position;
	Vector3 loot_travel_position;

	RecycledSequence recycledSequence = new RecycledSequence();
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		transform.position = Vector3.up * -100f;

		respond_scope_off.enabled = false;
	}
#endregion

#region API
    public void Spawm( Vector3 spawnPoint, int gunIndex )
    {
		gun_index     = gunIndex;
		loot_mesh.mesh = CurrentLevelData.Instance.levelData.gun_data[ gunIndex ].gun_mesh;

		respond_scope_off.enabled = true;

		transform.position    = spawnPoint;
		transform.eulerAngles = Vector3.zero;
		loot_travel_position  = spawnPoint + Random.insideUnitCircle.ConvertV3_Z() * GameSettings.Instance.loot_spawn_radius;



		var sequence = recycledSequence.Recycle();

		sequence.Append( transform.DOJump(
			loot_travel_position, GameSettings.Instance.loot_spawn_jump_power, 1,
			GameSettings.Instance.ScopeDuration )
			.SetEase( GameSettings.Instance.loot_spawn_jump_ease )
		);
		sequence.Join( transform.DORotate( Vector3.up * 90, GameSettings.Instance.ScopeDuration ).SetEase( GameSettings.Instance.loot_spawn_jump_ease ) );
	}

	public void GoTowardsPlayer()
	{
		respond_scope_off.enabled = false;

		player_position = ( notif_player_transform.sharedValue as Transform ).position;
		player_position = player_position + Random.insideUnitCircle.ConvertV3() * GameSettings.Instance.loot_spawn_travel_random;

		loot_travel_progression = 0;


		var sequence = recycledSequence.Recycle( OnProgressionComplete );

		sequence.AppendInterval( GameSettings.Instance.loot_spawn_travel_duration );
		sequence.Append( DOTween.To( ReturnProgression, SetProgression, 1,
				GameSettings.Instance.loot_spawn_travel_duration )
				.OnUpdate( OnProgressionUpdate )
				.SetEase( GameSettings.Instance.loot_spawn_travel_ease )
		);
		sequence.Join( transform.DORotate( Vector3.zero, GameSettings.Instance.loot_spawn_travel_duration ).SetEase( GameSettings.Instance.loot_spawn_travel_ease ) );
	}
#endregion

#region Implementation
	void OnProgressionUpdate()
	{
		transform.position = Vector3.Lerp( loot_travel_position, player_position, recycledSequence.Sequence.ElapsedPercentage() );
	}

	void OnProgressionComplete()
	{
		event_gun_change.Raise( gun_index );
		transform.position = Vector3.up * -100f;
	}

	float ReturnProgression()
	{
		return loot_travel_progression;
	}

	void SetProgression( float value )
	{
		loot_travel_progression = value;
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
