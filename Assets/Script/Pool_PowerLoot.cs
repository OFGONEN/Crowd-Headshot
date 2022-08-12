/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "pool_loot_power", menuName = "FF/Data/Pool/Power Loot" ) ]
public class Pool_PowerLoot : ComponentPool< PowerLoot >
{
    public void Spawn( int count, Vector3 spawnPoint )
    {
        for( var i = 0; i < count; i++ )
			GetEntity().Spawm( spawnPoint );
	}
}
