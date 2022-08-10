/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class SharedPositionSetter : MonoBehaviour
{
#region Fields
    [ SerializeField ] Transform _transform;
    [ SerializeField ] SharedVector3 shared_data;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		shared_data.sharedValue = _transform.position;
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
