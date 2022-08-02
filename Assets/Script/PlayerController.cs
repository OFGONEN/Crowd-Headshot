/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
#region Fields
    UnityMessage onFingerUp;
    UnityMessage onFingerDown;
    Vector2Delegate onFingerDrag;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		EmptyDelegates();
	}
#endregion

#region API
    public void OnLevelStart()
    {
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
		var input = gameEvent.eventValue.Invert(); // Vertical input affects X rotation axis & Horizontal input affetcs Y rotation axis
		onFingerDrag( input );
	}
#endregion

#region Implementation
    void FingerUp()
    {
		onFingerUp   = ExtensionMethods.EmptyMethod;
		onFingerDown = FingerDown;
		onFingerDrag = ExtensionMethods.EmptyMethod;
    }

	void FingerDown()
	{
		onFingerUp   = FingerUp;
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerDrag = FingerDrag;
	}

    void FingerDrag( Vector2 value )
    {
    }

    void EmptyDelegates()
    {
		onFingerUp   = ExtensionMethods.EmptyMethod;
		onFingerDown = ExtensionMethods.EmptyMethod;
		onFingerDrag = ExtensionMethods.EmptyMethod;
	}
#endregion

	#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
