/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "pool_loot_power", menuName = "FF/Data/Pool/Power Loot" ) ]
public class Pool_PowerLoot : ComponentPool< PowerLoot >
{
    public void Spawn( int power, Vector3 spawnPoint )
    {
		var divisor = CurrentLevelData.Instance.levelData.enemy_power_divisor;
		var count = power / divisor;
		var mod   = power % divisor;

		for( var i = 0; i < count; i++ )
			GetEntity().Spawm( spawnPoint, divisor );
        
		if( power < divisor || mod > 0 )
			GetEntity().Spawm( spawnPoint, power );
	}
}
