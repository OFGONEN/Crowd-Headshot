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

	int gun_index;
	float loot_travel_progression;
	Vector3 player_position;
	Vector3 loot_travel_position;

	RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void Spawm( Vector3 spawnPoint, int gunIndex )
    {
		gameObject.SetActive( true );

		gun_index     = gunIndex;
		loot_mesh.mesh = CurrentLevelData.Instance.levelData.gun_data[ gunIndex ].gun_mesh;

		transform.position   = spawnPoint;
		loot_travel_position = spawnPoint + Random.insideUnitCircle.ConvertV3_Z() * GameSettings.Instance.loot_spawn_radius;

		recycledTween.Recycle( transform.DOJump(
			loot_travel_position, GameSettings.Instance.loot_spawn_jump_power, 1,
			GameSettings.Instance.ScopeDuration )
			.SetEase( GameSettings.Instance.loot_spawn_jump_ease )
		);
	}

	public void GoTowardsPlayer()
	{
		player_position = ( notif_player_transform.sharedValue as Transform ).position;
		player_position = player_position + Random.insideUnitCircle.ConvertV3() * GameSettings.Instance.loot_spawn_travel_random;

		loot_travel_progression = 0;

		recycledTween.Recycle( 
			DOTween.To( ReturnProgression, SetProgression, 1,
				GameSettings.Instance.loot_spawn_travel_duration )
				.OnUpdate( OnProgressionUpdate )
				.SetEase( GameSettings.Instance.loot_spawn_travel_ease )
				.SetDelay( GameSettings.Instance.loot_spawn_travel_duration ), 
				OnProgressionComplete );
	}
#endregion

#region Implementation
	void OnProgressionUpdate()
	{
		transform.position = Vector3.Lerp( loot_travel_position, player_position, recycledTween.Tween.ElapsedPercentage() );
	}

	void OnProgressionComplete()
	{
		event_gun_change.Raise( gun_index );
		gameObject.SetActive( false );
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
