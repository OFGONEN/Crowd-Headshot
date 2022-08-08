/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class AnimationParameterRandomizer : MonoBehaviour
{
#region Fields
    [ SerializeField ] Animator animator;
    [ SerializeField ] AnimatorRandom_Integer[] animator_random_integer;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    private void Awake()
    {
		Randomize();
	}
#endregion

#region Implementation
    [ Button() ]
    void Randomize()
    {
        for( var i = 0; i < animator_random_integer.Length; i++ )
        {
			animator.SetInteger( animator_random_integer[ i ].parameter_name, Random.Range( 0, animator_random_integer[ i ].parameter_max ) );
		}
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}