/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
#region Fields
  [ Title( "Setup" ) ]
    [ SerializeField ] public bool enemy_is_walking;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        if( !enemy_is_walking )
			transform.forward = GameSettings.Instance.game_play_axis * -1f;
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
