/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields (Settings)
    // Info: You can use Title() attribute ONCE for every game-specific group of settings.
    
    [ Title( "Camera" ) ]
        [ LabelText( "Zoom In Value" ) ] public float camera_zoom_in = 2.8f;
        [ LabelText( "Zoom Out Value" ) ] public float camera_zoom_out = 2.8f;
        [ LabelText( "Zoom Speed" ), SuffixLabel( "units/seconds" ), Min( 0 ) ] public float camera_zoom_speed = 2.8f;
        [ LabelText( "Zoom Ease" ) ] public Ease camera_zoom_ease;
        [ LabelText( "Rotation Speed" ), SuffixLabel( "units/seconds" ), Min( 0 ) ] public float camera_rotation_speed = 1f;
        [ LabelText( "Rotation Default Duration" ) ] public float camera_rotation_duration = 1f;
        [ LabelText( "Rotation Clamp Horizontal" ) ] public Vector2 camera_rotation_clamp_horizontal;
        [ LabelText( "Rotation Clamp Vertical" ) ] public Vector2 camera_rotation_clamp_vertical;
        [ LabelText( "Default Rotation" ) ] public Vector3 camera_rotation_default;
        [ LabelText( "Follow Speed (Z)" ), SuffixLabel( "units/seconds" ), Min( 0 ) ] public float camera_follow_speed_depth = 2.8f;
        [ LabelText( "Shake Strenght" ) ] public float camera_shake_strenght = 15f;
        [ LabelText( "Shake Duration" ) ] public float camera_shake_duration = 0.35f;
    
    [ Title( "Player" ) ]
        [ LabelText( "Player Max Shoot Distance" ) ] public float player_shoot_maxDistance = 200f;
        [ LabelText( "Player Gun Aim Duration" ) ] public float player_aim_duration = 1;
        [ LabelText( "Player Move Speed" ) ] public float player_move_speed = 5;
        [ LabelText( "Player Move Ease" ) ] public Ease player_move_ease;

    [ Title( "Player Gun Change" ) ]
        [ LabelText( "Player Gun Default Scale" ) ] public float player_gun_change_default_scale;
        [ LabelText( "Player Gun Change Shrink Scale" ) ] public float player_gun_change_shrink_scale;
        [ LabelText( "Player Gun Change Shrink Duration" ) ] public float player_gun_change_shrink_duration;
        [ LabelText( "Player Gun Change Shrink Ease" ) ] public Ease player_gun_change_shrink_ease;
        [ LabelText( "Player Gun Change Grow Scale" ) ] public float player_gun_change_grow_scale;
        [ LabelText( "Player Gun Change Grow Duration" ) ] public float player_gun_change_grow_duration;
        [ LabelText( "Player Gun Change Grow Ease" ) ] public Ease player_gun_change_grow_ease;

    [ Title( "Enemy" ) ]
        [ LabelText( "Enemy Turn Animation Duration" ) ] public float enemy_animation_turn_duration = 1;
        [ LabelText( "Enemy Power Color Strong" ) ] public Color enemy_power_color_strong;
        [ LabelText( "Enemy Power Color Weak" ) ] public Color enemy_power_color_weak;
        [ LabelText( "Enemy HeadShot particle offset" ) ] public float enemy_particle_offset = 0.2f;
        [ LabelText( "Enemy Hit Force" ) ] public float enemy_hit_force = 5;

    [ Title( "Loot" ) ]
        [ LabelText( "Loot Spawn Radius" ) ] public float loot_spawn_radius = 1f;
        [ LabelText( "Loot Spawn Jump Power" ) ] public float loot_spawn_jump_power = 1f;
        [ LabelText( "Loot Spawn Jump Ease" ) ] public Ease loot_spawn_jump_ease;
        [ LabelText( "Loot Spawn Travel Duration" ) ] public float loot_spawn_travel_duration = 0.35f;
        [ LabelText( "Loot Spawn Travel Random" ) ] public float loot_spawn_travel_random = 0.35f;
        [ LabelText( "Loot Spawn Travel Ease" ) ] public Ease loot_spawn_travel_ease;

    [ Title( "Game UI" ) ]
        [ LabelText( "Crosshair Shoot Scale" ) ] public float ui_crosshair_shoot_scale;
        [ LabelText( "Crosshair Shoot Rotation" ) ] public float ui_crosshair_shoot_rotation;
        [ LabelText( "Crosshair Shoot Duration On" ) ] public float ui_crosshair_shoot_duration_on;
        [ LabelText( "Crosshair Shoot Duration Off" ) ] public float ui_crosshair_shoot_duration_off;
        [ LabelText( "Crosshair Shoot Ease On" ) ] public Ease ui_crosshair_shoot_ease_on;
        [ LabelText( "Crosshair Shoot Ease Off" ) ] public Ease ui_crosshair_shoot_ease_off;

    [ Title( "Game" ) ]
        [ LabelText( "Game-Play Axis" ) ] public Vector3 game_play_axis;
        [ LabelText( "Time Scale On Headshot" ) ] public float game_timeScale_headshot = 0.5f;
        [ LabelText( "Vignette Effect Value" ) ] public float game_vignette_value;
        [ LabelText( "Vignette Effect Duration" ) ] public float game_vignette_duration;
        [ LabelText( "Vignette Effect Ease" ) ] public Ease game_vignette_ease;
    
    [ Title( "Project Setup", "These settings should not be edited by Level Designer(s).", TitleAlignments.Centered ) ]
        public int maxLevelCount;
        
        // Info: 3 groups below (coming from template project) are foldout by design: They should remain hidden.
		[ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_GameSettings;
        [ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_Components;

        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text relative float height"                ) ] public float ui_PopUp_height;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text float duration"                       ) ] public float ui_PopUp_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Random Spawn Area in Screen" ), SuffixLabel( "percentage" ) ] public float ui_particle_spawn_width; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Duration" ), SuffixLabel( "seconds" ) ] public float ui_particle_spawn_duration; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Ease" ) ] public Ease ui_particle_spawn_ease;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Wait Time Before Target" ) ] public float ui_particle_target_waitTime;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Time" ) ] public float ui_particle_target_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Ease" ) ] public Ease ui_particle_target_ease;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;

        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_height;
        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_duration;

        public float ScopeDuration => ui_crosshair_shoot_duration_on + ui_crosshair_shoot_duration_off;
#endregion

#region Fields (Singleton Related)
        static GameSettings instance;

        delegate GameSettings ReturnGameSettings();
        static ReturnGameSettings returnInstance = LoadInstance;

		public static GameSettings Instance => returnInstance();
#endregion

#region Implementation
        static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
    }
}
