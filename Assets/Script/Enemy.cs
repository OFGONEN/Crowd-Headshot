/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
    [ SerializeField ] SharedVector3 shared_level_position_left;
    [ SerializeField ] SharedVector3 shared_level_position_right;
    [ SerializeField ] SharedFloatNotifier notif_player_power;

  [ Title( "Setup" ) ]
    [ SerializeField ] int enemy_power;
    [ SerializeField ] bool enemy_is_walking;
    [ SerializeField, ShowIf( "enemy_is_walking" ) ] bool enemy_walking_right;
    [ SerializeField, ShowIf( "enemy_is_walking" ) ] float enemy_walking_speed;

  [ Title( "Setup" ) ]
    [ SerializeField ] Transform enemy_gfx_transform;
    [ SerializeField ] TextMeshProUGUI enemy_text_power;
    [ SerializeField ] Animator enemy_animator;

	public Vector3 TeleportPosition => enemy_gfx_transform.position;
	public float Power => enemy_power;

// Private
	Collider[] enemy_collider;
	RecycledSequence recycledSequence = new RecycledSequence();
// Delegates
    Vector3 enemy_position;
    ReturnPosition returnTargetPosition;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		ToggleColliders( false );

		enemy_position = transform.position;

		enemy_text_power.text  = enemy_power.ToString();
		enemy_text_power.color = GameSettings.Instance.enemy_power_color_strong;

		if( enemy_is_walking )
        {
			Vector3 targetPosition;

			if( enemy_walking_right )
            {
				targetPosition = ReturnTargetPosition_Right();
				returnTargetPosition = ReturnTargetPosition_Right;
            }
            else
            {
				targetPosition = ReturnTargetPosition_Left();
				returnTargetPosition = ReturnTargetPosition_Left;
            }
            
		    enemy_gfx_transform.forward = targetPosition - enemy_gfx_transform.position;

			CreateWalkingSequence();
        }
		else // Stay Idle facing the Player
			enemy_gfx_transform.forward = GameSettings.Instance.game_play_axis * -1f;
	}
#endregion

#region API
	public void OnLevelStart()
	{
		ToggleColliders( true );
		OnPlayerPowerChange();
	}

	public void OnPlayerPowerChange()
	{
		if( notif_player_power.sharedValue <= enemy_power )
			enemy_text_power.color = GameSettings.Instance.enemy_power_color_weak;
		else
			enemy_text_power.color = GameSettings.Instance.enemy_power_color_strong;
	}

	public void Die()
	{
		recycledSequence.Kill();
		ToggleColliders( false );
		enemy_text_power.gameObject.SetActive( false );

		enemy_animator.SetTrigger( "die" );

	}
#endregion

#region Implementation
    void CreateWalkingSequence()
    {
		var targetPosition = returnTargetPosition();

		var duration = Vector3.Distance( enemy_gfx_transform.position, targetPosition ) / enemy_walking_speed; // X = V * t;

		var sequence = recycledSequence.Recycle( CreateWalkingSequence ); // Endless Loop

		sequence.Append( enemy_gfx_transform.DOMove( targetPosition, duration ).SetEase( Ease.Linear ) );
		sequence.AppendCallback( EnemyGFXTurn );
		sequence.AppendInterval( GameSettings.Instance.enemy_animation_turn_duration );
	}

    Vector3 ReturnTargetPosition_Right()
    {
		returnTargetPosition = ReturnTargetPosition_Left;
		return new Vector3( shared_level_position_right.sharedValue.x, 0, enemy_position.z );
	}

	Vector3 ReturnTargetPosition_Left()
	{
		returnTargetPosition = ReturnTargetPosition_Right;
		return new Vector3( shared_level_position_left.sharedValue.x, 0, enemy_position.z );
	}

    void EnemyGFXTurn()
    {
		enemy_animator.SetTrigger( "turn" );
	}

	void ToggleColliders( bool value )
	{
		foreach( var collider in enemy_collider )
			collider.enabled = value;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
