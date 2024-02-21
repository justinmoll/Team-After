using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour{

	public enum DrawMode
	{
		NoiseMap,
		ColorMap,
		Mesh
	}

	public AnimationCurve meshHeightCurve;
	public float meshHeightMultiplier;
	public DrawMode drawMode;
	public int mapWidth;
	public int mapHeight;
	public float noiseScale;
	public int octaves;
	public float lacunarity;
	[Range(0,1)]
	public float persistence;
	public bool autoUpdate;
	public int seed;
	public Vector2 offset;

	public TerrainType[] regions;
	// calls our generate noise map function
	public void GenerateMap(){
		float[,] noiseMap = Noise.generateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset, seed);
		
		Color[] colorMap = new Color[mapHeight * mapWidth];
		for (int y = 0; y < mapHeight; y++)
		{
			for (int x = 0; x < mapWidth; x++)
			{
				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < regions.Length; i++)
				{
					if (currentHeight <= regions[i].height)
					{
						colorMap[y * mapWidth + x] = regions[i].color;
						break;
					}
				}
			}
		}
		//creates a new display class.
		MapDisplay display = FindObjectOfType<MapDisplay>();

		if (drawMode == DrawMode.NoiseMap)
		{
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if (drawMode == DrawMode.ColorMap)
		{
			display.DrawTexture(TextureGenerator.textureFromColorMap(colorMap, mapWidth, mapHeight));
		}
		else if (drawMode == DrawMode.Mesh)
		{
			display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve),
				TextureGenerator.textureFromColorMap(colorMap, mapWidth, mapHeight));
		}
		//draws the noise map.
		
	}

	//clamp the octaves, map width, map height, and lacunarity. 
	void OnValidate()
	{
		if (mapWidth < 1) {
			mapWidth = 1;
		}

		if (mapHeight < 1) {
			mapHeight = 1;
		}

		if (octaves < 0) {
			octaves = 0;
		}

		if (lacunarity < 1) {
			lacunarity = 1;
		}
	}

	[System.Serializable]
	public struct TerrainType
	{
		public string name;
		public float height;
		public Color color;
	}
}


