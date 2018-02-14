using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.World;
using Assets.Scripts.World.Chunk;

public class ChunkManager : MonoBehaviour {
    public GameObject chunkPrefab;

    public MeshFilter meshFilter;
    private NoiseGenerator noiseGen;
    private List<Chunk> chunksToGenerate;
    private List<Chunk> chunksBeingGenerated;
    private List<Chunk> chunksGenerated;
    private List<Task<Chunk>> tasks;
    private byte maxGenTasks = 1;

    // Use this for initialization
    void Start() {
        chunksToGenerate = new List<Chunk>();
        chunksBeingGenerated = new List<Chunk>();
        chunksGenerated = new List<Chunk>();
        tasks = new List<Task<Chunk>>(maxGenTasks);
        noiseGen = new NoiseGenerator(60f);
        
        for (int x = 0; x < 12; x++) {
            for (int z = 0; z < 12; z++) {
                Chunk chunk = new Chunk(new Vector2Int(x, z), CoordinateSpace.Chunk);
                chunk.CreateInstances(chunkPrefab);
                chunksToGenerate.Add(chunk);
            }
        }
    }
	
    private void UpdateTasks() {
        for (int i = tasks.Count - 1; i >= 0; i--) {
            Task<Chunk> task = tasks[i];

            if (task.IsCompleted) {
                tasks[i].Result.UpdateChunkMesh();
                tasks.RemoveAt(i);
            }
        }

        while (chunksToGenerate.Count > 0 && tasks.Count < maxGenTasks) {
            CreateAndStartChunkTask();
        }
    }

    private void CreateAndStartChunkTask() {
        if (chunksToGenerate.Count > 0) {
            Chunk chunk = chunksToGenerate[0];
            Task<Chunk> task = new Task<Chunk>(() => {
                chunk.GenerateTerrainAndMarch(noiseGen);
                return chunk;
            });

            chunksBeingGenerated.Add(chunk);
            chunksToGenerate.RemoveAt(0);
            tasks.Add(task);
            task.Start();
        }
    }

    // Update is called once per frame
    void Update () {
        UpdateTasks();
	}
}
