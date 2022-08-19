/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class UIManager : MonoBehaviour
    {
#region Fields
    [ Title( "Event Listeners" ) ]
        public EventListenerDelegateResponse levelLoadedResponse;
        public EventListenerDelegateResponse levelCompleteResponse;
        public EventListenerDelegateResponse levelFailResponse;
        public EventListenerDelegateResponse tapInputListener;

    [ Title( "UI Elements" ) ]
        public UI_Patrol_Scale level_loadingBar_Scale;
        public TextMeshProUGUI level_count_text;
        public TextMeshProUGUI level_information_text;
        public UI_Patrol_Scale level_information_text_Scale;
        public Image loadingScreenImage;
        public Image foreGroundImage;
        public RectTransform tutorialObjects;
        public RectTransform parent_level_progress;
		public RectTransform parent_tapToStart;
		public Image parent_level_finished;

    [ Title( "UI Scope Elements" ) ]
        public GameObject parent_scope;
        public RectTransform rect_scope_mask;
        public RectTransform rect_scope_background;
        public RectTransform rect_scope_crosshair;
        public GameObject parent_scope_hit;

    [ Title( "Fired Events" ) ]
        public GameEvent levelRevealedEvent;
        public GameEvent loadNewLevelEvent;
        public GameEvent resetLevelEvent;
        public GameEvent event_level_started;
        public ElephantLevelEvent elephantLevelEvent;
        public SharedBool shared_hit;

    [ Title( "Resources" ) ]
		public Sprite sprite_level_complete;
		public Sprite sprite_level_failed;
    
    RecycledSequence recycledSequence = new RecycledSequence();
#endregion

#region Unity API
        private void OnEnable()
        {
            levelLoadedResponse.OnEnable();
            levelFailResponse.OnEnable();
            levelCompleteResponse.OnEnable();
            tapInputListener.OnEnable();
        }

        private void OnDisable()
        {
            levelLoadedResponse.OnDisable();
            levelFailResponse.OnDisable();
            levelCompleteResponse.OnDisable();
            tapInputListener.OnDisable();
        }

        private void Awake()
        {
            levelLoadedResponse.response   = LevelLoadedResponse;
            levelFailResponse.response     = LevelFailResponse;
            levelCompleteResponse.response = LevelCompleteResponse;
            tapInputListener.response      = ExtensionMethods.EmptyMethod;

			level_information_text.text = string.Empty;
			parent_level_finished.gameObject.SetActive( false );
		}
#endregion

#region API
        public void OnScopeOn()
        {
			rect_scope_background.sizeDelta = Vector2.one * 10000;

			rect_scope_crosshair.eulerAngles = Vector3.zero;
			rect_scope_crosshair.localScale  = Vector3.one;
			rect_scope_mask.localScale       = Vector3.one;

			parent_level_progress.gameObject.SetActive( false );
			parent_scope.SetActive( true );
		}

        public void OnScopeOff()
        {
			parent_level_progress.gameObject.SetActive( true );
			parent_scope.SetActive( false );
        }

        public void OnScopeShoot()
        {
			var duration_on = GameSettings.Instance.ui_crosshair_shoot_duration_on;
			var ease_on     = GameSettings.Instance.ui_crosshair_shoot_ease_on;

			var duration_off = GameSettings.Instance.ui_crosshair_shoot_duration_off;
			var ease_off = GameSettings.Instance.ui_crosshair_shoot_ease_off;

			var sequence = recycledSequence.Recycle();
			if( shared_hit.sharedValue )
				sequence.AppendCallback( EnableCrosshairHit );
			sequence.Append( rect_scope_crosshair.DOScale(
				GameSettings.Instance.ui_crosshair_shoot_scale, duration_on )
				.SetEase( ease_on ) );
 			sequence.Join( rect_scope_mask.DOScale(
				GameSettings.Instance.ui_crosshair_shoot_scale, duration_on )
				.SetEase( ease_on ) );
			sequence.Join( rect_scope_crosshair.DORotate(
				GameSettings.Instance.ui_crosshair_shoot_rotation * Vector3.one, duration_on )
				.SetEase( ease_on ) );
			sequence.AppendCallback( DisableCrosshairHit );
			sequence.Append( rect_scope_crosshair.DOScale(
				1, duration_off )
				.SetEase( ease_off ) );
			sequence.Join( rect_scope_mask.DOScale(
				1, duration_off )
				.SetEase( ease_off ) );
			sequence.Join( rect_scope_crosshair.DORotate(
				Vector3.one, duration_off )
				.SetEase( ease_off ) );
		}
#endregion

#region Implementation
        private void LevelLoadedResponse()
        {
			level_information_text.text = string.Empty;

			var sequence = DOTween.Sequence()
								.Append( level_loadingBar_Scale.DoScale_Target( Vector3.zero, GameSettings.Instance.ui_Entity_Scale_TweenDuration ) )
								.Append( loadingScreenImage.DOFade( 0, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
								.Join( foreGroundImage.DOFade( 0, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
								.AppendCallback( () => tapInputListener.response = StartLevel );

			level_count_text.text = "Level " + CurrentLevelData.Instance.currentLevel_Shown;

            levelLoadedResponse.response = NewLevelLoaded;
        }

        private void NewLevelLoaded()
        {
			level_count_text.text = "Level " + CurrentLevelData.Instance.currentLevel_Shown;
			level_information_text.text = string.Empty;

			parent_tapToStart.gameObject.SetActive( true );
			parent_tapToStart.localScale = Vector3.one;

			var sequence = DOTween.Sequence();

			// Tween tween = null;

			sequence.Append( foreGroundImage.DOFade( 0f, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
					// .Append( tween ) // TODO: UIElements tween.
					// .Append( level_information_text_Scale.DoScale_Start( GameSettings.Instance.ui_Entity_Scale_TweenDuration ) )
					.AppendCallback( () => tapInputListener.response = StartLevel );

            // elephantLevelEvent.level             = CurrentLevelData.Instance.currentLevel_Shown;
            // elephantLevelEvent.elephantEventType = ElephantEvent.LevelStarted;
            // elephantLevelEvent.Raise();
        }

        private void LevelCompleteResponse()
        {
            var sequence = DOTween.Sequence();

			// Tween tween = null;

			parent_level_finished.sprite = sprite_level_complete;
			parent_level_finished.gameObject.SetActive( true );
			
			level_information_text.text = "Tap to Continue";

			sequence.Append( foreGroundImage.DOFade( 0.5f, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
					// .Append( tween ) // TODO: UIElements tween.
					.Append( level_information_text_Scale.DoScale_Start( GameSettings.Instance.ui_Entity_Scale_TweenDuration ) )
					.AppendCallback( () => tapInputListener.response = LoadNewLevel );

            elephantLevelEvent.level             = CurrentLevelData.Instance.currentLevel_Shown;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelCompleted;
            elephantLevelEvent.Raise();
        }

        private void LevelFailResponse()
        {
            var sequence = DOTween.Sequence();

			// Tween tween = null;
			parent_level_finished.sprite = sprite_level_failed;
			parent_level_finished.gameObject.SetActive( true );

			level_information_text.text = "Tap to Continue";

			sequence.Append( foreGroundImage.DOFade( 0.5f, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
                    // .Append( tween ) // TODO: UIElements tween.
					.Append( level_information_text_Scale.DoScale_Start( GameSettings.Instance.ui_Entity_Scale_TweenDuration ) )
					.AppendCallback( () => tapInputListener.response = Resetlevel );

            elephantLevelEvent.level             = CurrentLevelData.Instance.currentLevel_Shown;
            elephantLevelEvent.elephantEventType = ElephantEvent.LevelFailed;
            elephantLevelEvent.Raise();
        }



		private void StartLevel()
		{
			foreGroundImage.DOFade( 0, GameSettings.Instance.ui_Entity_Fade_TweenDuration );

			// level_information_text_Scale.DoScale_Target( Vector3.zero, GameSettings.Instance.ui_Entity_Scale_TweenDuration );
			// level_information_text_Scale.Subscribe_OnComplete( () => 
			// {
			// 	levelRevealedEvent.Raise();
			// 	event_level_started.Raise();
			// } );

			parent_tapToStart.DOScale( 0, GameSettings.Instance.ui_Entity_Scale_TweenDuration )
				.OnComplete( () =>
				{
					levelRevealedEvent.Raise();
					event_level_started.Raise();
				} );

			tutorialObjects.gameObject.SetActive( false );

			tapInputListener.response = ExtensionMethods.EmptyMethod;

			elephantLevelEvent.level             = CurrentLevelData.Instance.currentLevel_Shown;
			elephantLevelEvent.elephantEventType = ElephantEvent.LevelStarted;
			elephantLevelEvent.Raise();
		}

		private void LoadNewLevel()
		{
			parent_level_finished.gameObject.SetActive( false );

			tapInputListener.response = ExtensionMethods.EmptyMethod;

			var sequence = DOTween.Sequence();

			sequence.Append( foreGroundImage.DOFade( 1f, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
			        .Join( level_information_text_Scale.DoScale_Target( Vector3.zero, GameSettings.Instance.ui_Entity_Scale_TweenDuration ) )
			        .AppendCallback( loadNewLevelEvent.Raise );
		}

		private void Resetlevel()
		{
			parent_level_finished.gameObject.SetActive( false );

			tapInputListener.response = ExtensionMethods.EmptyMethod;

			var sequence = DOTween.Sequence();

			sequence.Append( foreGroundImage.DOFade( 1f, GameSettings.Instance.ui_Entity_Fade_TweenDuration ) )
			        .Join( level_information_text_Scale.DoScale_Target( Vector3.zero, GameSettings.Instance.ui_Entity_Scale_TweenDuration ) )
			        .AppendCallback( resetLevelEvent.Raise );
		}

		void EnableCrosshairHit()
		{
			parent_scope_hit.SetActive( enabled );
		}

		void DisableCrosshairHit()
		{
			parent_scope_hit.SetActive( false );
		}
#endregion
    }
}