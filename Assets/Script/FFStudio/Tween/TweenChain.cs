/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Shapes;
using UnityEditor;
#endif

namespace FFStudio
{
    public class TweenChain : MonoBehaviour
    {
#region Fields (Inspector Interface)
	[ Title( "Setup" ) ]
		[ InfoBox( "Transform of this GO will be used unless another one is provided.", "TransformIsNull" ) ]
		[ SerializeField, LabelText( "Target Transform" ) ] Transform transform_target;

    [ Title( "Start Options" ) ]
        public bool playOnStart = false;
        [ LabelText( "Index to Play on Start" ), ShowIf( "playOnStart" ) ] public int index_toPlayOnStart;
        
    [ Title( "Tween Data" ) ]
#if UNITY_EDITOR
	[ TableList( ShowIndexLabels = true ) ]
#endif
	[ SerializeReference ]
        public List< TweenData > tweenDatas = new List< TweenData >();
		
		Transform transform_ToTween;
#endregion

#region Properties
        [ ShowInInspector, ReadOnly ]
		public bool IsPlaying => index_playing >= 0;
		public bool IsPaused { get; private set; }

		int index_playing;
#endregion

#region Unity API
        void Awake()
        {
			index_playing = -1;

			transform_ToTween = transform_target == null ? transform : transform_target;

			foreach( var tweenData in tweenDatas )
				tweenData.Initialize( transform_ToTween );
		}

        void Start()
        {
            if( !enabled )
                return;

            if( playOnStart )
                Play( index_toPlayOnStart );
        }
#endregion

#region API
        [ Button() ]
        public void Play( int index )
        {
#if UNITY_EDITOR
            if( tweenDatas == null || tweenDatas.Count == 0 )
                FFLogger.LogError( name + ": Tween data array is null or has no elements! Fix this before build!", this );
            else if( index < 0 || index > tweenDatas.Count - 1 )
				FFLogger.LogError( name + ": Given index {index} is outside tween data array's range! Fix this before build!", this );
#endif
			if( IsPlaying )
				tweenDatas[ index ].Kill();

			index_playing = index;
			var tweenData = tweenDatas[ index ];
            
            if( tweenData.chain )
			    tweenData.Play( () => ChainNext( index ) );
            else
			    tweenData.Play( OnComplete );
		}
		
		[ Button(), EnableIf( "IsPaused" ) ]
        public void Play()
        {
			tweenDatas[ index_playing ].Play();
			IsPaused = false;
		}
        
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Pause()
        {
			tweenDatas[ index_playing ].Pause();
			IsPaused = true;
		}
		
        [ Button(), EnableIf( "IsPlaying" ) ]
        public void Stop()
        {
			tweenDatas[ index_playing ].Stop();
		}
		
		[ Button() ]
        public void Kill()
        {
			tweenDatas[ index_playing ].Kill();
		}
#endregion

#region Implementation
        void ChainNext( int indexOfPlayingTweenData )
        {
#if UNITY_EDITOR
			if( IsPlaying )
				inPlayMode_currentStartPos = transform_ToTween.position;
#endif
			var tweenData = tweenDatas[ indexOfPlayingTweenData ];
			if( IsPlaying && tweenData.chain )
				Play( tweenData.index_nextUp );
		}
		
		void OnComplete()
		{
			index_playing = -1;
		}
#endregion

#region EditorOnly
#if UNITY_EDITOR
		bool TransformIsNull => transform_target == null;

		Vector3 inPlayMode_currentStartPos;
		GUIStyle style;

		void DrawMovementTweenGizmo( MovementTweenData tweenData, ref Vector3 lastPos, Vector3 verticalOffset, int tweenNo )
		{
			Vector3 startPos = lastPos;

			if( Application.isPlaying )
				startPos = inPlayMode_currentStartPos;

			Color color = new Color( 1.0f, 0.75f, 0.0f );

			Draw.UseDashes = true;
			Draw.DashStyle = DashStyle.RelativeDashes( DashType.Basic, 1, 1 );

			var deltaPosition = tweenData.endValue;

			lastPos = startPos + deltaPosition;

			Draw.Line( startPos + verticalOffset, lastPos + verticalOffset, 0.1f, LineEndCap.None, color );
			var direction = deltaPosition.normalized;
			var deltaMagnitude = deltaPosition.magnitude;
			var coneLength = 0.2f;
			var conePos = Vector3.Lerp( startPos, lastPos, 1.0f - coneLength / deltaMagnitude );
			Draw.Cone( conePos + verticalOffset, deltaPosition.normalized, 0.1f, 0.2f, color );
			Handles.Label( ( lastPos + startPos ) / 2 + verticalOffset, tweenNo.ToString() + ": " + tweenData.description, style );
		}

		void OnDrawGizmosSelected()
		{
			var theTransform = transform_target == null ? transform : transform_target;

			style = new GUIStyle { normal = new GUIStyleState { textColor = Color.red }, fontSize = 20 };

			Vector3 lastPos = theTransform.position;
			if( Application.isPlaying )
			{
				if( IsPlaying == false )
					return;

				var tweenData = tweenDatas[ index_playing ];

				if( tweenData is MovementTweenData )
					DrawMovementTweenGizmo( tweenData as MovementTweenData, ref lastPos, Vector3.zero, index_playing + 1 );
			}
			else
				for( var i = 0; i < tweenDatas.Count; i++ )
					if( tweenDatas[ i ] is MovementTweenData )
						DrawMovementTweenGizmo( tweenDatas[ i ] as MovementTweenData, ref lastPos, Vector3.up * i * 0.3f, i + 1 );
		}
#endif
#endregion
	}
}
