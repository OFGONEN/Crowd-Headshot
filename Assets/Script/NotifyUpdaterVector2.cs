/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;
using Sirenix.OdinInspector;

public class NotifyUpdaterVector2 : MonoBehaviour
{
#region Fields
	[ Title( "Setup" ) ]
		[ SerializeField ] SharedVector2Notifier sharedDataNotifier;
		[ SerializeField ] UnityEvent< Vector2 > notify_event;
#endregion

#region Properties
#endregion

#region Unity API
		void OnEnable()
		{
			sharedDataNotifier.Subscribe( OnSharedDataChange );
		}

		void OnDisable()
		{
			sharedDataNotifier.Unsubscribe( OnSharedDataChange );
		}
#endregion

#region API
#endregion

#region Implementation
		void OnSharedDataChange()
		{
			notify_event.Invoke( sharedDataNotifier.SharedValue );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
