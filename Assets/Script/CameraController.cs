/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
#region Fields
  [ Title( "Setup" ) ]
    [ SerializeField ] Camera _camera;
	[ SerializeField ] CameraZoomNotifier notif_camera_zoom;
	[ SerializeField ] CameraRotationNotifier notif_camera_rotation;
#endregion

#region Properties
#endregion

#region Unity API
	private void Awake()
	{
		notif_camera_zoom.sharedValue     = _camera.fieldOfView;
		notif_camera_rotation.sharedValue = GameSettings.Instance.camera_rotation_default;
	}
#endregion

#region API
    public void OnCameraZoomLevelChange( float value )
    {
		_camera.fieldOfView = value;
	}

    public void OnCameraRotateChange( Vector2 value )
    {
		transform.localEulerAngles = value;
	}

	public void OnLevelFailed()
	{
		_camera.DOShakeRotation( GameSettings.Instance.camera_shake_duration, GameSettings.Instance.camera_shake_strenght );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}