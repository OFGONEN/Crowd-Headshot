/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PowerLoot : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
	[ SerializeField ] SharedFloatNotifier notif_player_power;
	[ SerializeField ] SharedReferenceNotifier notif_player_transform;
    [ SerializeField ] Pool_PowerLoot pool_loot_power;

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
    public void Spawm( Vector3 spawnPoint )
    {
		gameObject.SetActive( true );

		transform.position = spawnPoint;
		loot_travel_position = spawnPoint + Random.insideUnitCircle.ConvertV3_Z() * GameSettings.Instance.loot_spawn_radius;

		recycledTween.Recycle( transform.DOJump( loot_travel_position, GameSettings.Instance.loot_spawn_jump_power, 1, GameSettings.Instance.loot_spawn_jump_duration ) );
	}

	public void GoTowardsPlayer()
	{
		player_position = ( notif_player_transform.sharedValue as Transform ).position;
		loot_travel_progression = 0;

		recycledTween.Recycle( 
			DOTween.To( ReturnProgression, SetProgression, 1,
				GameSettings.Instance.ui_crosshair_shoot_duration_on + GameSettings.Instance.ui_crosshair_shoot_duration_off )
				.OnUpdate( OnProgressionUpdate ) , OnProgressionComplete );
	}
#endregion

#region Implementation
	void OnProgressionUpdate()
	{
		transform.position = Vector3.Lerp( loot_travel_position, player_position, recycledTween.Tween.ElapsedPercentage() );
	}

	void OnProgressionComplete()
	{
		FFLogger.Log( "OnProgressionComplete" );
		notif_player_power.SharedValue += 1;
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