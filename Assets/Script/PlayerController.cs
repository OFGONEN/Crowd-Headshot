/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
	[ SerializeField ] SharedFloatNotifier notif_player_power;
    [ SerializeField ] CameraRotationNotifier notif_camera_rotation;
    [ SerializeField ] CameraZoomNotifier notif_camera_zoom;
    [ SerializeField ] SharedReferenceNotifier notif_camera_reference;
	[ SerializeField ] SharedFloatNotifier notif_vignette_intensity;

  [ Title( "Fired Events" ) ]
	[ SerializeField ] ParticleSpawnEvent event_particle;
	[ SerializeField ] GameEvent event_level_failed;

    Transform camera_transform;
	int player_layerMask;

	UnityMessage onFingerUp;
    UnityMessage onFingerDown;
    Vector2Delegate onFingerDrag;

    RecycledSequence recycledSequence = new RecycledSequence();
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		// Set player power to 1
		notif_player_power.SetValue_DontNotify( 1 );

		EmptyDelegates();
		player_layerMask = ~LayerMask.GetMask( "Player_Combat", "Enemy_Combat", "Boss_Combat", "Ground" );
	}
#endregion

#region API
    public void OnLevelStart()
    {
		camera_transform = notif_camera_reference.sharedValue as Transform;
		onFingerDown = FingerDown;
	}

    public void OnLevelFinished()
    {
		EmptyDelegates();
	}

    public void OnFingerUp()
    {
		onFingerUp();
	}

    public void OnFingerDown()
    {
		onFingerDown();
	}

    public void OnFingerDrag( Vector2GameEvent gameEvent )
    {
		onFingerDrag( gameEvent.eventValue );
	}
#endregion

#region Implementation
    void FingerUp()
    {
		EmptyDelegates();

		var sequence = recycledSequence.Recycle( OnZoomedOut );
		sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
		sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
		sequence.AppendInterval( Mathf.Max( GameSettings.Instance.camera_rotation_duration, notif_camera_zoom.CurrentDuration_ZoomOut() ) );
	}

	void FingerUp_Shoot()
	{
		FingerUp();

		RaycastHit hitInfo;
		var hit = Physics.Raycast( camera_transform.position, camera_transform.forward, out hitInfo, GameSettings.Instance.player_shoot_maxDistance, player_layerMask );

		if( hit )
		{
			Component attachedComponent = null;
			attachedComponent = hitInfo.collider.GetComponent< TriggerListener_Enter >()?.AttachedComponent;

			DamageEnemy( hitInfo.point, attachedComponent );
		}
	}

	void FingerDown()
	{
        // Handle delegates
		onFingerUp   = FingerUp;
		onFingerDown = ExtensionMethods.EmptyMethod;

		//todo sniper gun animation
		var sequence = recycledSequence.Recycle( OnZoomedIn );
		sequence.AppendCallback( notif_camera_zoom.OnZoomIn );
		sequence.AppendInterval( notif_camera_zoom.CurrentDuration_ZoomIn() );
	}

    void FingerDrag( Vector2 value )
    {
		var input = new Vector2( -value.y, value.x );
		notif_camera_rotation.OnFingerDrag( input * GameSettings.Instance.camera_rotation_speed * Time.deltaTime );
	}

    void OnZoomedIn()
    {
		onFingerUp   = FingerUp_Shoot;
		onFingerDrag = FingerDrag;
	}

    void OnZoomedOut()
    {
		onFingerDown = FingerDown;
	}

    void EmptyDelegates()
    {
		onFingerUp   = ExtensionMethods.EmptyMethod;
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerDrag = ExtensionMethods.EmptyMethod;
	}

	void DamageEnemy( Vector3 hitPosition, Component component )
	{
		event_particle.Raise( "hit", hitPosition );

		if( component is Enemy )
		{
			var enemy = component as Enemy;
			var enemyPower = enemy.Power;

			if( notif_player_power.sharedValue >= enemyPower )
			{
				enemy.Die();
				transform.position = enemy.TeleportPosition;

				notif_player_power.SharedValue += enemyPower;
			}
			else
				LevelFailed();
		}
	}

	void LevelFailed()
	{
		EmptyDelegates();

		var sequence = recycledSequence.Recycle();
		sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
		sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
		sequence.AppendInterval( Mathf.Max( GameSettings.Instance.camera_rotation_duration, notif_camera_zoom.CurrentDuration_ZoomOut() ) );
		// todo: Vignette effect callback
		// todo: Vignette effect add interval 
		sequence.AppendCallback( event_level_failed.Raise );
	}
#endregion

	#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
