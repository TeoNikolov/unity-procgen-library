using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.World;
using Assets.Scripts.World.Chunk;

public class ChunkManager : MonoBehaviour {

    public GameObject chunkPrefab;

    //DEBUG
    public MeshFilter meshFilter;
    private List<ChunkInstance> chunkInstanceList;
    private NoiseGenerator noiseGen;

    // Use this for initialization
    void Start() {
        noiseGen = new NoiseGenerator(60f);

        chunkInstanceList = new List<ChunkInstance>();
        
        for (int x = 0; x < 8; x++) {
            for (int z = 0; z < 8; z++) {
                Chunk chunk = new Chunk(new Vector2Int(x, z), CoordinateSpace.Chunk);
                chunk.CreateInstances(chunkPrefab);
                chunk.GenerateTerrainAndMarch(noiseGen);
            }
        }

        //Voxel[] voxels = new Voxel[4096];
        //float scale = 20.0f;
        //float[] heightMap = new float[17 * 17];
        //Random RNG = new Random();

        //for (int x = 0; x < 17; x++) {
        //    for (int y = 0; y < 17; y++) {
        //        float sampleX = x / scale;
        //        float sampleY = y / scale;
        //        float value = Mathf.PerlinNoise(sampleX, sampleY) * 17;
        //        heightMap[x + y * 17] = value;
        //    }
        //}

        //for (int y = 0; y < 16; y++) {
        //    for (int z = 0; z < 16; z++) {
        //        for (int x = 0; x < 16; x++) {
        //            Voxel voxel = new Voxel();
        //            voxel.SetCorner(0, y <= heightMap[x + z * 17]);
        //            voxel.SetCorner(1, y <= heightMap[x + 1 + z * 17]);
        //            voxel.SetCorner(2, y <= heightMap[x + 1 + (z + 1) * 17]);
        //            voxel.SetCorner(3, y <= heightMap[x + (z + 1) * 17]);
        //            voxel.SetCorner(4, y + 1 <= heightMap[x + z * 17]);
        //            voxel.SetCorner(5, y + 1 <= heightMap[x + 1 + z * 17]);
        //            voxel.SetCorner(6, y + 1 <= heightMap[x + 1 + (z + 1) * 17]);
        //            voxel.SetCorner(7, y + 1 <= heightMap[x + (z + 1) * 17]);
        //            voxels[x + z * 16 + y * 256] = voxel;
        //        }
        //    }
        //}

        //meshFilter.mesh = MarchingCubes.March(new Vector3Int(16, 16, 16), voxels);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
