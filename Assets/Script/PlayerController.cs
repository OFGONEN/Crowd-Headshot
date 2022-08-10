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
	[ SerializeField ] GameEvent event_level_completed;
	[ SerializeField ] GameEvent event_level_failed;
	[ SerializeField ] GameEvent event_scope_on;
	[ SerializeField ] GameEvent event_scope_shoot;
	[ SerializeField ] GameEvent event_scope_off;

  [ Title( "Components" ) ]
	[ SerializeField ] Animator player_animator;

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
	private void OnDisable()
	{
		recycledSequence.Kill();
	}

    private void Awake()
    {
		// Set player power to 1
		notif_player_power.SetValue_NotifyAlways( 1 );

		EmptyDelegates();
		player_layerMask = LayerMask.GetMask( "Enemy_Combat", "Boss_Combat", "Ground" );
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
		sequence.AppendCallback( event_scope_off.Raise );
		sequence.AppendCallback( PlayerGunDown );
		sequence.AppendInterval( GameSettings.Instance.player_aim_duration );
	}

	void FingerUp_Shoot()
	{
		EmptyDelegates();

		RaycastHit hitInfo;
		var hit = Physics.Raycast( camera_transform.position, camera_transform.forward, out hitInfo, GameSettings.Instance.player_shoot_maxDistance, player_layerMask );

		if( hit )
		{
			// Spawn hit particle effect
			var particleRotation = Quaternion.LookRotation( ( transform.position - hitInfo.point ).normalized, Vector3.up ).eulerAngles.SetX( 0 ).SetZ( 0 );
			event_particle.Raise( "hit", hitInfo.point, particleRotation );

			var triggerListener = hitInfo.collider.GetComponent< TriggerListener_Enter >();

			if( triggerListener != null && triggerListener.AttachedComponent is Enemy )
			{
				var enemy      = triggerListener.AttachedComponent as Enemy;
				var enemyPower = enemy.Power;

				if( notif_player_power.sharedValue >= enemyPower )
				{
					enemy.Die();
					notif_player_power.SharedValue += enemyPower;

					if( enemy.IsBoss )
					{
						var sequence = recycledSequence.Recycle();
						sequence.AppendCallback( event_scope_shoot.Raise );
						sequence.AppendInterval( GameSettings.Instance.ui_crosshair_shoot_duration_on + GameSettings.Instance.ui_crosshair_shoot_duration_off );
						sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
						sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
						sequence.AppendInterval( Mathf.Max( GameSettings.Instance.camera_rotation_duration, notif_camera_zoom.CurrentDuration_ZoomOut() ) );
						sequence.AppendCallback( event_scope_off.Raise );
						sequence.AppendCallback( PlayerGunDown );
						sequence.AppendCallback( event_level_completed.Raise );
					}
					else
					{
						PlayerScopeOffSequence();

						// Teleport to enemy position
						transform.position = enemy.TeleportPosition.SetY( 0 );
					}
				}
				else
					LevelFailed();
			}
			else
				PlayerScopeOffSequence();
		}
		else
			PlayerScopeOffSequence();
	}

	void FingerDown()
	{
        // Handle delegates
		onFingerUp   = FingerUp;
		onFingerDown = ExtensionMethods.EmptyMethod;

		var sequence = recycledSequence.Recycle( OnZoomedIn );
		sequence.AppendCallback( PlayerGunUp );
		sequence.AppendInterval( GameSettings.Instance.player_aim_duration );
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

		event_scope_on.Raise();
	}

    void OnZoomedOut()
    {
		onFingerDown = FingerDown;
	}

	void PlayerGunUp()
	{
		player_animator.SetBool( "up", true );
	}

	void PlayerGunDown()
	{
		player_animator.SetBool( "up", false );
	}

    void EmptyDelegates()
    {
		onFingerUp   = ExtensionMethods.EmptyMethod;
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerDrag = ExtensionMethods.EmptyMethod;
	}

	void PlayerScopeOffSequence()
	{
		var sequence = recycledSequence.Recycle( OnZoomedOut );
		sequence.AppendCallback( event_scope_shoot.Raise );
		sequence.AppendInterval( GameSettings.Instance.ui_crosshair_shoot_duration_on + GameSettings.Instance.ui_crosshair_shoot_duration_off );
		sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
		sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
		sequence.AppendInterval( Mathf.Max( GameSettings.Instance.camera_rotation_duration, notif_camera_zoom.CurrentDuration_ZoomOut() ) );
		sequence.AppendCallback( event_scope_off.Raise );
		sequence.AppendCallback( PlayerGunDown );
		sequence.AppendInterval( GameSettings.Instance.player_aim_duration );
	}

	void LevelFailed()
	{
		var sequence = recycledSequence.Recycle();
		sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
		sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
		sequence.AppendInterval( Mathf.Max( GameSettings.Instance.camera_rotation_duration, notif_camera_zoom.CurrentDuration_ZoomOut() ) );
		sequence.Join( DOTween.To( GetVignette, SetVignette,
			GameSettings.Instance.game_vignette_value, GameSettings.Instance.game_vignette_duration ).SetEase( GameSettings.Instance.game_vignette_ease ) );
		sequence.AppendCallback( event_scope_off.Raise );
		sequence.AppendInterval( GameSettings.Instance.game_vignette_duration );
		sequence.AppendCallback( event_level_failed.Raise );
	}

	float GetVignette()
	{
		return notif_vignette_intensity.sharedValue;
	}

	void SetVignette( float value )
	{
		notif_vignette_intensity.SharedValue = value;
	}
#endregion

	#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
