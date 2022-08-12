/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PowerLoot : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
    [ SerializeField ] SharedVector3 shared_level_position_left;
    [ SerializeField ] SharedVector3 shared_level_position_right;
    [ SerializeField ] Pool_PowerLoot pool_loot_power;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnDisable()
    {
    }
#endregion

#region API
    public void Spawm( Vector3 spawnPoint )
    {
		transform.position = spawnPoint;
		var jumpPoint = spawnPoint + Random.insideUnitCircle.ConvertV3_Z() * GameSettings.Instance.loot_spawn_radius;

		transform.DOJump( jumpPoint, GameSettings.Instance.loot_spawn_jump_power, 1, GameSettings.Instance.loot_spawn_jump_duration );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
