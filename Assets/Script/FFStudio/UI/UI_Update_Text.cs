/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class UI_Update_Text< SharedDataNotifierType, SharedDataType > : MonoBehaviour
        where SharedDataNotifierType : SharedDataNotifier< SharedDataType >
    {
#region Fields
    [ Title( "Setup" ) ]
        [ SerializeField ] protected SharedDataNotifierType sharedDataNotifier;

        protected TextMeshProUGUI ui_Text; 
#endregion

#region Unity API
        void OnEnable()
        {
            sharedDataNotifier.Subscribe( OnSharedDataChange );
            OnSharedDataChange();
        }
        
        void OnDisable()
        {
            sharedDataNotifier.Unsubscribe( OnSharedDataChange );
        }

        void Awake()
        {
            ui_Text = GetComponentInChildren< TextMeshProUGUI >();
        }
#endregion

#region Base Class API
        protected virtual void OnSharedDataChange()
        {
			ui_Text.text = sharedDataNotifier.SharedValue.ToString();
        }
#endregion
    }
}
