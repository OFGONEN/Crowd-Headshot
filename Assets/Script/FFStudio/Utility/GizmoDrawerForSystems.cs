/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

public class GizmoDrawerForSystems : MonoBehaviour
{
#if UNITY_EDITOR
	// Info: Example usage:
	// [ SerializeField ] XXXSystem system_xxx;

	public void OnDrawGizmos()
    {
		// Info: Example usage:
		// system_xxx?.DrawGizmos();
	}
#endif
}
