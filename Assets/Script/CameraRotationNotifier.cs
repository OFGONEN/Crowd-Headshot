/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "notif_camera_rotation", menuName = "FF/Game/Camera Rotation Notifier" ) ]
public class CameraRotationNotifier : SharedVector2Notifier
{
#region Fields
    RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void OnFingerDrag( Vector2 value )
    {
		SharedValue = new Vector2(
			Mathf.Clamp( sharedValue.x + value.x, GameSettings.Instance.camera_rotation_clamp_vertical.x, GameSettings.Instance.camera_rotation_clamp_vertical.y ),
			Mathf.Clamp( sharedValue.y + value.y, GameSettings.Instance.camera_rotation_clamp_horizontal.x, GameSettings.Instance.camera_rotation_clamp_horizontal.y ) );
	}

    public void OnDefaultRotation()
    {
		recycledTween.Recycle( DOTween.To( GetValue, SetValue, GameSettings.Instance.camera_rotation_default, GameSettings.Instance.camera_rotation_duration ) );
	}
#endregion

#region Implementation
    Vector2 GetValue()
    {
		return sharedValue;
	}

    void SetValue( Vector2 value )
    {
		SetValue_NotifyAlways( value );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
