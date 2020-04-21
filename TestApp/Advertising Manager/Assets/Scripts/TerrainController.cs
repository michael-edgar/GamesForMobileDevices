/*
 * Copyright (C) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using UnityEngine;
using System.Collections;
using System;

// handles the terrain data for the level.
public class TerrainController : MonoBehaviour
{
	
	protected Terrain  terrain;
	protected TerrainData tData;
	protected int terrainXRes;
	protected int terrainZRes;
	
	// 16 levels 0-F (hex)
	protected float mStrength = 1 / 16.0f;
	protected float posMinX;
	protected float posMaxX ;
	protected float posMinY;
	protected float posMaxY ;
	protected float posMinZ ;
	protected float posMaxZ ;
	
	
	// Use this for initialization
	virtual public void Start ()
	{
		terrain = transform.GetComponent<Terrain> (); 
		tData = terrain.terrainData;
		
		terrainXRes = tData.heightmapWidth;
		terrainZRes = tData.heightmapHeight;
		posMinX = terrain.transform.position.x;
		posMaxX = terrain.terrainData.size.x;
		posMinY = terrain.transform.position.y;
		posMaxY = terrain.terrainData.size.y;
		posMinZ = terrain.transform.position.z;
		posMaxZ = terrain.terrainData.size.z;
		
		ResetLevel ();
	}
	
	virtual public void ResetLevel ()
	{
		
	}
	
	public void reset ()
	{
		float[,] heights = TerrainHeights;
		for (int z = 0; z < terrainZRes; z++) {
			for (int x = 0; x < terrainXRes; x++) {
				heights [x, z] = 0f;
			}
		}
		TerrainHeights = heights;
	}
	
	public float GetHeightForSymbol (char ch)
	{
		float height;
		if (ch == '#') {
			//height = 1.0f;
			height = 0.0f;
		} else if (ch >= '0' && ch <= '9') {
			height = mStrength * (Convert.ToInt16 (ch) - 48);
		} else if (ch >= 'A' && ch <= 'F') {
			height = mStrength * (10 + (Convert.ToInt16 (ch) - 65));
		} else {
			height = -1f;
		}
		return height;
	}
	
	public float[,] TerrainHeights {
		get {
			return tData.GetHeights (0, 0, terrainXRes, terrainZRes);
		}
		
		set {
			tData.SetHeights (0, 0, value);
			tData.RefreshPrototypes ();
			//	UpdateSplatMap();
			terrain.Flush ();
		}
	}
	
	public Vector3 PosMax {
		get {
			return new Vector3 (posMaxX, posMaxY, posMaxZ);
		}
	}
	
	public Vector3 PosMin {
		get {
			return new Vector3 (posMinX, posMinY, posMinZ);
		}
	}
	
	public LevelManager LevelInfo {
		
		get {
			return GameManager.Instance.LevelInfo;
		}
	}

}

