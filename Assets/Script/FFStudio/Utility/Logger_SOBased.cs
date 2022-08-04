/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
    [ CreateAssetMenu( fileName = "util_logger", menuName = "FF/Utility/Logger" ) ]
    public class Logger_SOBased : ScriptableObject
    {
#region Fields
        [ ShowInInspector ] Transform target_transform; // Info: Used for both context & Popup Texts.
        [ ShowInInspector ] GameObject target_gameObject; // Info: Used for context.
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
        public void Log( string text )
        {
            FFLogger.Log( text );
        }
        
        public void Log_WithContext_Transform( string text )
        {
            FFLogger.Log( text, target_transform );
        }

		public void Log_WithContext_GameObject( string text )
		{
			FFLogger.Log( text, target_gameObject );
		}
        
        public void SetTarget( Transform transform )
        {
			target_transform = transform;
		}

		public void SetTarget( GameObject gameObject )
		{
			target_gameObject = gameObject;
		}
        
        public void Log_Popup_Transform( string text )
        {
			FFLogger.PopUpText( target_transform.position, text );
		}

		public void Log_Popup_GameObject( string text )
		{
			FFLogger.PopUpText( target_gameObject.transform.position, text );
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
    }
}