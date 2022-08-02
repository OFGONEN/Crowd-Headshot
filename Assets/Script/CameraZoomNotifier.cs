/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;

[ CreateAssetMenu( fileName = "notif_camera_zoom", menuName = "FF/Game/Camera Zoom Notifier" ) ]
public class CameraZoomNotifier : SharedFloatNotifier
{
#region Fields
    RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    public void OnZoomIn()
    {
		recycledTween.Recycle( 
            DOTween.To( GetZoomValue, SetZoomValue, GameSettings.Instance.camera_zoom_in, GameSettings.Instance.camera_zoom_speed )
            .SetSpeedBased()
            .SetEase( GameSettings.Instance.camera_zoom_ease )
        );
	}

    public void OnZoomOut()
    {
		recycledTween.Recycle( 
            DOTween.To( GetZoomValue, SetZoomValue, GameSettings.Instance.camera_zoom_out, GameSettings.Instance.camera_zoom_speed )
            .SetSpeedBased()
            .SetEase( GameSettings.Instance.camera_zoom_ease )
        );
    }
#endregion

#region Implementation
    float GetZoomValue()
    {
		return sharedValue;
	}

	void SetZoomValue( float value )
	{
		SetValue_NotifyAlways( value );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
