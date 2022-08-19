/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_CutoutMask : Image
{
#region Fields
#endregion

#region Properties
    public override Material materialForRendering
    {
		get
		{
			Material material = new Material( base.materialForRendering );
			material.SetInt( "_StencilComp", ( int )CompareFunction.NotEqual );
			return material;
		}
	}
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion
}
