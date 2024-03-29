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
	[ SerializeField ] SharedBool shared_hit;

  [ Title( "Fired Events" ) ]
	[ SerializeField ] ParticleSpawnEvent event_particle;
	[ SerializeField ] GameEvent event_level_completed;
	[ SerializeField ] GameEvent event_level_failed;
	[ SerializeField ] GameEvent event_scope_on;
	[ SerializeField ] GameEvent event_scope_shoot;
	[ SerializeField ] GameEvent event_scope_off;
	[ SerializeField ] GameEvent event_camera_shake;

  [ Title( "Components" ) ]
	[ SerializeField ] Animator player_animator;
	[ SerializeField ] MeshFilter player_gun_mesh;
	[ SerializeField ] ParticleSystem particle_speed_trail;
	[ SerializeField ] ParticleSystem particle_gun_change;

    Transform camera_transform;
	int player_layerMask;
	int player_gun_index;

	UnityMessage onFingerUp;
    UnityMessage onFingerDown;
    Vector2Delegate onFingerDrag;

    RecycledSequence recycledSequence = new RecycledSequence();
    RecycledSequence recycledSequence_GunChange = new RecycledSequence();
#endregion

#region Properties
#endregion

#region Unity API
	private void OnDisable()
	{
		recycledSequence.Kill();
		recycledSequence_GunChange.Kill();
	}

    private void Awake()
    {
		// Set player power to 1
		player_gun_index = 0;

		notif_player_power.SetValue_NotifyAlways( CurrentLevelData.Instance.levelData.player_power_level );
		player_gun_mesh.mesh = CurrentLevelData.Instance.levelData.gun_data[ 0 ].gun_mesh;

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

	public void ChangeGun( IntGameEvent gameEvent )
	{
		player_gun_index = gameEvent.eventValue;

		var sequence = recycledSequence_GunChange.Recycle();

		sequence.Append( player_gun_mesh.transform.DOScale(
			GameSettings.Instance.player_gun_change_shrink_scale,
			GameSettings.Instance.player_gun_change_shrink_duration )
			.SetEase( GameSettings.Instance.player_gun_change_shrink_ease )
		);
		sequence.AppendCallback( () => ChangeGunMesh( player_gun_index ) );
		sequence.AppendCallback( particle_gun_change.Play );
		sequence.Append( player_gun_mesh.transform.DOPunchScale(
			Vector3.one * GameSettings.Instance.player_gun_change_grow_scale,
			GameSettings.Instance.player_gun_change_grow_duration )
			.SetEase( GameSettings.Instance.player_gun_change_grow_ease )
		);
	}
#endregion

#region Implementation
	void ChangeGunMesh( int index )
	{
		// particle_gun_change.Play( true );
		player_gun_mesh.mesh = CurrentLevelData.Instance.levelData.gun_data[ index ].gun_mesh;
		player_gun_mesh.transform.localScale = Vector3.one * GameSettings.Instance.player_gun_change_default_scale;
	}

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
					var weaponDropped = enemy.Die();

					triggerListener.AttachedRigidbody.AddForce( 
						( hitInfo.point - transform.position ).normalized * GameSettings.Instance.enemy_hit_force, 
						ForceMode.Impulse );

					// notif_player_power.SharedValue += enemyPower;
					// OnPlayerPowerChange( notif_player_power.sharedValue );
					shared_hit.sharedValue = true;

					if( triggerListener.tag == "Head" )
					{
						event_particle.Raise( "hit_head", hitInfo.point + Vector3.up * GameSettings.Instance.enemy_particle_offset, Vector3.zero );
						Time.timeScale = GameSettings.Instance.game_timeScale_headshot;
						DOVirtual.DelayedCall( GameSettings.Instance.ui_crosshair_shoot_duration_on, () => Time.timeScale = 1, true );
					}

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
						PlayerKilledEnemySequence( enemy.TeleportPosition.SetY( 0 ), weaponDropped );
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

		shared_hit.sharedValue = false;

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

	void PlayerKilledEnemySequence( Vector3 position, bool weaponLoot )
	{
		float moveDelay = 0;

		if( weaponLoot )
			moveDelay = Mathf.Max( GameSettings.Instance.player_aim_duration, GameSettings.Instance.loot_spawn_travel_duration ) + GameSettings.Instance.loot_spawn_travel_duration;
		else
			moveDelay = Mathf.Max( GameSettings.Instance.player_aim_duration, GameSettings.Instance.loot_spawn_travel_duration );

		var duration = Vector3.Distance( transform.position, position ) / GameSettings.Instance.player_move_speed;
		var sequence = recycledSequence.Recycle( OnZoomedOut );
		sequence.AppendCallback( event_scope_shoot.Raise );
		sequence.AppendInterval( GameSettings.Instance.ui_crosshair_shoot_duration_on + GameSettings.Instance.ui_crosshair_shoot_duration_off );
		sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
		sequence.AppendInterval( notif_camera_zoom.CurrentDuration_ZoomOut() );
		sequence.AppendCallback( event_scope_off.Raise );
		sequence.AppendCallback( PlayerGunDown );
		sequence.AppendInterval( moveDelay );
		sequence.AppendCallback( () => particle_speed_trail.Play( true ) );
		sequence.Append( transform.DOMove( position, duration ).SetEase( GameSettings.Instance.player_move_ease ) );
		sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
		sequence.AppendCallback( () => particle_speed_trail.Stop( true, ParticleSystemStopBehavior.StopEmitting ) );
		sequence.AppendInterval( GameSettings.Instance.camera_rotation_duration );
	}

	void LevelFailed()
	{
		var zoomOutDuration = Mathf.Max( GameSettings.Instance.game_vignette_duration, GameSettings.Instance.ui_crosshair_shoot_duration_on + GameSettings.Instance.ui_crosshair_shoot_duration_off );

		var sequence = recycledSequence.Recycle();
		sequence.AppendCallback( event_scope_shoot.Raise );
		sequence.AppendCallback( event_camera_shake.Raise );
		sequence.Join( DOTween.To( GetVignette, SetVignette,
			GameSettings.Instance.game_vignette_value, GameSettings.Instance.game_vignette_duration ).SetEase( GameSettings.Instance.game_vignette_ease ) );
		sequence.AppendInterval( zoomOutDuration );
		sequence.AppendCallback( notif_camera_zoom.OnZoomOut );
		sequence.AppendCallback( notif_camera_rotation.OnDefaultRotation );
		sequence.AppendInterval( Mathf.Max( GameSettings.Instance.camera_rotation_duration, notif_camera_zoom.CurrentDuration_ZoomOut() ) );
		sequence.AppendCallback( event_scope_off.Raise );
		sequence.AppendCallback( PlayerGunDown );
		sequence.AppendInterval( GameSettings.Instance.player_aim_duration );
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
