/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class MovementTweenData : TweenData
	{
		public enum MovementMode { Local, World }

#region Fields
	[ Title( "Movement Tween" ) ]
		[ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ) ] public bool useDelta = true;
#if UNITY_EDITOR
		[ InfoBox( "End Value is RELATIVE.", "useDelta" ) ]
		[ InfoBox( "End Value is ABSOLUTE.", "EndValueIsAbsolute" ) ]
#endif
		[ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ) ] public Vector3 endValue;
#if UNITY_EDITOR
		[ InfoBox( "Duration is DURATION.", "useDelta" ) ]
		[ InfoBox( "Duration is VELOCITY.", "EndValueIsAbsolute" ) ]
#endif
		[ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ) ] public float duration;
		[ BoxGroup( "Tween" ), PropertyOrder( int.MinValue ) ] public MovementMode movementMode;
#endregion

#region Properties
#if UNITY_EDITOR
		bool EndValueIsAbsolute => !useDelta;
#endif
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
		protected override void CreateAndStartTween( UnityMessage onComplete, bool isReversed = false )
		{
			var duration = useDelta ? Mathf.Abs( endValue.magnitude / this.duration ) : this.duration;

			if( movementMode == MovementMode.Local )
				recycledTween.Recycle( transform.DOLocalMove( isReversed ? -endValue : endValue, duration ), onComplete );
			else
				recycledTween.Recycle( transform.DOMove( isReversed ? -endValue : endValue, duration ), onComplete );

			recycledTween.Tween
				.SetLoops( loop ? -1 : 0, loopType )
				.SetEase( easing );
				
			if( useDelta )
				recycledTween.Tween.SetRelative();

#if UNITY_EDITOR
			recycledTween.Tween.SetId( "_ff_movement_tween___" + description );
#endif

			base.CreateAndStartTween( onComplete, isReversed );
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
#endif
#endregion
	}
}