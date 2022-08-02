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
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
