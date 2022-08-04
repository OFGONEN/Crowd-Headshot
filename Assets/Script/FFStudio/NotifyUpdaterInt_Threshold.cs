/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class NotifyUpdaterInt_Threshold : NotifyUpdater< SharedIntNotifier, int >
    {
		public override bool HideBaseClassOnNotifyEvent => true;

		[ SerializeField ] int threshold = 0;
		
		[ HideIf( "NoEventsPopulatedYet" ) ]
		public bool showUnusedEvents = false;
		
        [ SerializeField, ShowIf( "ShowOnBelowEvent" ) ] UnityEvent< int > onBelow;
		[ SerializeField, ShowIf( "ShowOnEqualOrBelowEvent" ) ] UnityEvent< int > onEqualorBelow;
		[ SerializeField, ShowIf( "ShowOnEqualEvent" ) ] UnityEvent< int > onEqual;
		[ SerializeField, ShowIf( "ShowOnEqualOrAboveEvent" ) ] UnityEvent< int > onEqualorAbove;
        [ SerializeField, ShowIf( "ShowOnAboveEvent" ) ] UnityEvent< int > onAbove;
        
		protected override void OnSharedDataChange()
		{
            if( sharedDataNotifier.sharedValue < threshold )
			{
				onBelow.Invoke( sharedDataNotifier.sharedValue );
				onEqualorBelow.Invoke( sharedDataNotifier.sharedValue );
			}
				
			if( sharedDataNotifier.sharedValue == threshold )
			{
				onEqualorBelow.Invoke( sharedDataNotifier.sharedValue );
				onEqual.Invoke( sharedDataNotifier.sharedValue );
				onEqualorAbove.Invoke( sharedDataNotifier.sharedValue );
			}

			if( sharedDataNotifier.sharedValue > threshold )
			{
				onAbove.Invoke( sharedDataNotifier.sharedValue );
				onEqualorAbove.Invoke( sharedDataNotifier.sharedValue );
			}
		}

		bool ShowOnBelowEvent()        => NoEventsPopulatedYet() || showUnusedEvents || onBelow.GetPersistentEventCount() 			> 0;
		bool ShowOnEqualOrBelowEvent() => NoEventsPopulatedYet() || showUnusedEvents || onEqualorBelow.GetPersistentEventCount() 	> 0;
		bool ShowOnEqualEvent()        => NoEventsPopulatedYet() || showUnusedEvents || onEqual.GetPersistentEventCount() 			> 0;
		bool ShowOnEqualOrAboveEvent() => NoEventsPopulatedYet() || showUnusedEvents || onEqualorAbove.GetPersistentEventCount() 	> 0;
		bool ShowOnAboveEvent()        => NoEventsPopulatedYet() || showUnusedEvents || onAbove.GetPersistentEventCount() 			> 0;
		
		bool NoEventsPopulatedYet()
		{
			return
				onBelow.GetPersistentEventCount() +
				onEqualorBelow.GetPersistentEventCount() +
				onEqual.GetPersistentEventCount() +
				onEqualorAbove.GetPersistentEventCount() +
				onAbove.GetPersistentEventCount() == 0;
		}
	}
}