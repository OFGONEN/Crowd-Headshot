/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
    [ SerializeField ] SharedVector3 shared_level_position_left;
    [ SerializeField ] SharedVector3 shared_level_position_right;

  [ Title( "Setup" ) ]
    [ SerializeField ] bool enemy_is_walking;
    [ SerializeField, ShowIf( "enemy_is_walking" ) ] bool enemy_walking_right;
    [ SerializeField, ShowIf( "enemy_is_walking" ) ] float enemy_walking_speed;

  [ Title( "Setup" ) ]
    [ SerializeField ] Transform enemy_gfx_transform;
    [ SerializeField ] Animator enemy_animator;

// Private
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
		enemy_position = transform.position;

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
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
