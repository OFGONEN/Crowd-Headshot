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
  [ Title( "Components" ) ]
    [ SerializeField ] Camera _camera;

    RecycledTween recycledTween = new RecycledTween();
#endregion

#region Properties
#endregion

#region Unity API
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

    public void OnDefaultRotation()
    {
		recycledTween.Recycle( transform.DOLocalRotate( 
            GameSettings.Instance.camera_rotation_default, 
            GameSettings.Instance.camera_rotation_speed
            ).SetSpeedBased() 
        );
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}