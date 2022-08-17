/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

public class UIPowerUpText : MonoBehaviour
{
#region Fields
  [ Title( "Setup" ) ]
    [ SerializeField ] float tween_duration;
    [ SerializeField ] Ease tween_movement_ease;
    [ SerializeField ] Ease tween_fade_ease;

  [ Title( "Components" ) ]
    [ SerializeField ] RectTransform _rectTransform;
    [ SerializeField ] RectTransform parent_target;
    [ SerializeField ] TextMeshProUGUI text_power;

    RecycledSequence recycledSequence = new RecycledSequence();

    Vector3 position_start;
#endregion

#region Properties
#endregion

#region Unity API
    private void Start()
    {
		position_start     = _rectTransform.position;
		text_power.enabled = false;
	}
#endregion

#region API
    public void OnPowerChange( IntGameEvent gameEvent )
    {
		_rectTransform.position = position_start;

		text_power.text = "+ " + gameEvent.eventValue;
		text_power.color = text_power.color.SetAlpha( 1 );
		text_power.enabled = true;

		var sequence = recycledSequence.Recycle();

		sequence.Append( _rectTransform.DOMove( parent_target.position, tween_duration ).SetEase( tween_movement_ease ) );
		sequence.Join( text_power.DOFade( 0, tween_duration ).SetEase( tween_fade_ease ) );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}