using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.World;
using Assets.Scripts.World.Chunk;

public class ChunkManager : MonoBehaviour {
    public GameObject chunkPrefab;

    /// <summary>
    /// The origin Transform based on which chunk loading and unloading is calculated.
    /// </summary>
    public Transform origin;
    public PlayerController playerController;

    /// <summary>
    /// A reference to the instance of this chunk manager. Only one chunk manager is allowed.
    /// </summary>
    public static ChunkManager instance;

    private NoiseGenerator noiseGen;
    private List<Tuple<int, int>> coordinateMap;
    private List<Chunk> chunksToGenerate;
    private List<Chunk> chunksBeingGenerated;
    private List<Chunk> chunksGenerated;
    private List<Chunk> chunksToRemove;
    private List<Task<Chunk>> tasks;
    private byte maxGenTasks = 1;

    // Use this for initialization
    void Start() {
        instance = this;
        Chunk.chunks = new Dictionary<ulong, Chunk>();
        coordinateMap = ComputeCoordinateMap();
        chunksToGenerate = new List<Chunk>();
        chunksBeingGenerated = new List<Chunk>();
        chunksGenerated = new List<Chunk>();
        chunksToRemove = new List<Chunk>();
        tasks = new List<Task<Chunk>>(maxGenTasks);
        noiseGen = new NoiseGenerator(60f);
        
        //for (int x = 0; x < 12; x++) {
        //    for (int z = 0; z < 12; z++) {
        //        Chunk chunk = new Chunk(new Vector2Int(x, z), CoordinateSpace.Chunk);
        //        chunk.CreateInstances(chunkPrefab);
        //        chunksToGenerate.Add(chunk);
        //    }
        //}
    }
	
    private void UpdateTasks() {
        for (int i = tasks.Count - 1; i >= 0; i--) {
            Task<Chunk> task = tasks[i];

            if (task.IsCompleted) {
                chunksBeingGenerated.Remove(task.Result);
                chunksGenerated.Add(task.Result);
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

    private void SortGenerationQueue(Tuple<int, int> sortOrigin) {
        chunksToGenerate.Sort(delegate (Chunk c1, Chunk c2) {
            int c1X = c1.coordinates.GetX(CoordinateSpace.Chunk);
            int c1Z = c1.coordinates.GetZ(CoordinateSpace.Chunk);
            int c2X = c2.coordinates.GetX(CoordinateSpace.Chunk);
            int c2Z = c2.coordinates.GetZ(CoordinateSpace.Chunk);
            double d1 = Distance(sortOrigin, new Tuple<int, int>(c1X, c1Z));
            double d2 = Distance(sortOrigin, new Tuple<int, int>(c2X, c2Z));
            return d1.CompareTo(d2);
        });
    }

    private void OffsetCoordinateMap(int offset_x, int offset_z) {
        foreach (Tuple<int, int> t in coordinateMap) {
            int x = t.Item1 + offset_x;
            int z = t.Item2 + offset_z;
            Chunk chunk = Chunk.GetChunk(x, z, CoordinateSpace.Chunk);

            if (!chunk.isGenerated) {
                chunk.CreateInstances(chunkPrefab);
                chunksToGenerate.Add(chunk);
            }
        }
    }

    /// <summary>
    /// Specifies how often the generation of chunks should be updated.
    /// Includes: Calculating new chunks to generate, queueing them and sorting them according to current player location.
    /// </summary>
    private const double chunkGenUpdateConstant = 0.25f;

    /// <summary>
    /// The current timer used for chunk generation updates.
    /// </summary>
    private double chunkGenUpdateTimer = 0.0f;

    /// <summary>
    /// Specifies how often the removal of chunks should be updated.
    /// </summary>
    private const double chunkRemoveConstant = 0.25f;

    /// <summary>
    /// The current timer used for chunk removal updates.
    /// </summary>
    private double chunkRemoveTimer = 0.0f;

    /// <summary>
    /// The maximum number of chunks to remove at each chunk removal update.
    /// </summary>
    private const int chunkRemoveCount = 4;

    private void UpdateChunkGeneration() {
        int origin_x = (int)origin.position.x >> 4;
        int origin_z = (int)origin.position.z >> 4;
        OffsetCoordinateMap(origin_x, origin_z);
        SortGenerationQueue(new Tuple<int, int>(origin_x, origin_z));
    }

    private void UpdateChunkRemoval() {
        int playerX = ChunkCoordinates2D.ConvertSpace((int)origin.transform.position.x, CoordinateSpace.World);
        int playerZ = ChunkCoordinates2D.ConvertSpace((int)origin.transform.position.z, CoordinateSpace.World);

        for (int i = chunksToGenerate.Count - 1; i >= 0; i--) {
            ChunkCoordinates2D coordinates = chunksToGenerate[i].coordinates;
            int x = coordinates.GetX(CoordinateSpace.Chunk);
            int z = coordinates.GetZ(CoordinateSpace.Chunk);

            if (Vector2.Distance(new Vector2(x, z), new Vector2(playerX, playerZ)) > playerController.viewDistanceFurthest) {
                chunksToRemove.Add(chunksToGenerate[i]);
                chunksToGenerate.RemoveAt(i);
            }
        }

        for (int i = chunksGenerated.Count - 1; i >= 0; i--) {
            ChunkCoordinates2D coordinates = chunksGenerated[i].coordinates;
            int x = coordinates.GetX(CoordinateSpace.Chunk);
            int z = coordinates.GetZ(CoordinateSpace.Chunk);

            if (Vector2.Distance(new Vector2(x, z), new Vector2(playerX, playerZ)) > playerController.viewDistanceFurthest) {
                chunksToRemove.Add(chunksGenerated[i]);
                chunksGenerated.RemoveAt(i);
            }
        }

        int chunksRemoved = 0;

        while (chunksToRemove.Count > 0 && chunksRemoved < chunkRemoveCount) {
            chunksToRemove[0].Destroy();
            chunksToRemove.RemoveAt(0);
            chunksRemoved++;
        }

        //List<ulong> chunksToRemove = new List<ulong>();

        //foreach (KeyValuePair<ulong, Chunk> pair in Chunk.chunks) {
        //    ChunkCoordinates2D coordinates = pair.Value.coordinates;
        //    int x = coordinates.GetX(CoordinateSpace.Chunk);
        //    int z = coordinates.GetZ(CoordinateSpace.Chunk);
        //    int playerX = ChunkCoordinates2D.ConvertSpace((int)origin.transform.position.x, CoordinateSpace.World);
        //    int playerZ = ChunkCoordinates2D.ConvertSpace((int)origin.transform.position.z, CoordinateSpace.World);

        //    if (Vector2.Distance(new Vector2(x, z), new Vector2(playerX, playerZ)) > playerController.viewDistanceFurthest) {
        //        chunksToRemove.Add(pair.Key);
        //    }
        //}

        //int counter = 0;

        //foreach (ulong hash in chunksToRemove) {
        //    if (counter >= removeChunkCount) {
        //        break;
        //    }

        //    Chunk chunk;
        //    Chunk.chunks.TryGetValue(hash, out chunk);

        //    if (chunk != null) {
        //        chunk.Destroy();
        //        counter++;
        //    } else {
        //        Debug.Log("Tried retrieving a chunk with hash '" + hash + "', but got null.");
        //    }
        //}
    }

    private void UpdateChunks() {
        if (chunkGenUpdateTimer >= chunkGenUpdateConstant) {
            chunkGenUpdateTimer -= 0.25;
            UpdateChunkGeneration();
        }

        if (chunkRemoveTimer >= chunkRemoveConstant) {
            chunkRemoveTimer -= 0.25;
            UpdateChunkRemoval();
        }

        // Update Counters
        chunkGenUpdateTimer += Time.deltaTime;
        chunkRemoveTimer += Time.deltaTime;
    }

    // Update is called once per frame
    void Update () {
        UpdateChunks();
        UpdateTasks();
	}


    /// <summary>
    /// Compute a list of points in a circular area around an origin point (0, 0), sorted by the distance to specified point.
    /// The area's radius is the player's near view distance.
    /// </summary>
    /// <returns>The sorted list of points</returns>
    public List<Tuple<int, int>> ComputeCoordinateMap() {
        int radius = playerController.viewDistanceNear;
        int[,] grid = new int[radius * 2 + 1, radius * 2 + 1];
        Tuple<int, int> origin = new Tuple<int, int>(0, 0);
        List<Tuple<int, int>> points = new List<Tuple<int, int>>();

        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {
                points.Add(new Tuple<int, int>(x, y));
            }
        }

        for (int i = points.Count - 1; i >= 0; i--) {
            if (Vector2.Distance(new Vector2(0, 0), new Vector2(points[i].Item1, points[i].Item2)) > radius) {
                points.RemoveAt(i);
            }
        }

        points.Sort(delegate (Tuple<int, int> p1, Tuple<int, int> p2) {
            double d1 = Distance(origin, p1);
            double d2 = Distance(origin, p2);
            return d1.CompareTo(d2);
        });

        return points;
    }

    /// <summary>
    /// Evaluate the distance between two arbitary points.
    /// </summary>
    /// <param name="p1">The first point.</param>
    /// <param name="p2">The second point.</param>
    /// <returns>The distance between the two points.</returns>
    private static double Distance(Tuple<int, int> p1, Tuple<int, int> p2) {
        return Math.Sqrt(Math.Pow(p2.Item1 - p1.Item1, 2) + Math.Pow(p2.Item2 - p1.Item2, 2));
    }




}
