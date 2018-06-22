using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.World;
using Assets.Scripts.World.Chunk;
using Assets.Scripts.Utilities.PriorityQueue;

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
    private int maxChunks = 1000;
    private NoiseGenerator noisegen;
    private List<Tuple<int, int>> coordinateMapMin;
    private List<Tuple<int, int>> coordinateMapMax;
    private Dictionary<ulong, Chunk> c_generate;
    private Dictionary<ulong, Chunk> c_busy;
    private Dictionary<ulong, Chunk> c_done;
    private Dictionary<ulong, Chunk> c_remove;
    private Dictionary<ulong, Chunk> c_gBuffer;
    private Dictionary<ulong, Chunk> c_dBuffer;
    private List<ulong> keyBuffer;

    private List<Chunk> chunksToGenerate;
    private List<Chunk> chunksBeingGenerated;
    private List<Chunk> chunksGenerated;
    private List<Chunk> chunksToRemove;

    private PriorityQueue<Chunk> chunkQueue;
    private Dictionary<ulong, PriorityQueueNode<Chunk>> c_nodes;

    private List<Task<Chunk>> tasks;
    private byte maxGenTasks = 4;

    private static readonly int seed = 10000;

    // Use this for initialization
    void Start() {
        instance = this;
        //Chunk.chunks = new Dictionry<ulong, Chunk>();
        coordinateMapMin = ComputeCoordinateMap(playerController.viewDistanceMin);
        coordinateMapMax = ComputeCoordinateMap(playerController.viewDistanceMax);

        c_generate = new Dictionary<ulong, Chunk>();
        c_gBuffer = new Dictionary<ulong, Chunk>();
        c_dBuffer = new Dictionary<ulong, Chunk>();
        c_busy = new Dictionary<ulong, Chunk>();
        c_done = new Dictionary<ulong, Chunk>();
        c_remove = new Dictionary<ulong, Chunk>();
        c_nodes = new Dictionary<ulong, PriorityQueueNode<Chunk>>();
        chunkQueue = new PriorityQueue<Chunk>(maxChunks);
        keyBuffer = new List<ulong>(16);

        chunksToGenerate = new List<Chunk>();
        chunksBeingGenerated = new List<Chunk>();
        chunksGenerated = new List<Chunk>();
        chunksToRemove = new List<Chunk>();

        tasks = new List<Task<Chunk>>(maxGenTasks);
        noisegen = new NoiseGenerator(120f, -1, 3);
        this.playerController.OnChunkBorderCrossed += new OnChunkBorderCrossedHandler(UpdateChunkGeneration);
    }
	
    private void UpdateTasks() {
        for (int i = tasks.Count - 1; i >= 0; i--) {
            Task<Chunk> task = tasks[i];

            if (task.IsCompleted) {
                c_busy.Remove(task.Result.hashcode);
                c_done.Add(task.Result.hashcode, task.Result);
                task.Result.UpdateChunkMesh();
                tasks.RemoveAt(i);
            }
        }

        while (!chunkQueue.IsEmpty() && tasks.Count < maxGenTasks) {
            Chunk chunk = chunkQueue.Dequeue().data;

            Task<Chunk> task = new Task<Chunk>(() => {
                chunk.GenerateTerrainAndMarch(noisegen);
                return chunk;
            });

            c_nodes.Remove(chunk.hashcode);
            c_busy.Add(chunk.hashcode, chunk);
            c_generate.Remove(chunk.hashcode);
            tasks.Add(task);
            task.Start();
        }
    }

    private void OffsetCoordinateMap(int offset_x, int offset_z) {
        foreach (Tuple<int, int> t in coordinateMapMin) {
            int x = t.Item1 + offset_x;
            int z = t.Item2 + offset_z;
            Chunk chunk = Chunk.GetChunk(x, z, CoordinateSpace.Chunk);

            if (!chunk.isGenerated && !chunk.isBeingGenerated) {
                chunk.CreateInstances(chunkPrefab);
                chunksToGenerate.Add(chunk);
                chunk.isBeingGenerated = true;
            }
        }
    }

    /// <summary>
    /// Specifies how often the generation of chunks should be updated.
    /// Includes: Calculating new chunks to generate, queueing them and sorting them according to current player location.
    /// </summary>
    private const double chunkGenUpdateConstant = 0.1f;

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
    private const int chunkRemoveCount = 16;

    /// <summary>
    /// The generate (first) pass of the chunk manager's chunk update.
    /// It determines the chunks that need to be generated.
    /// It places the chunk in the correct queue (to generate, to remove, generated).
    /// </summary>
    /// <param name="offset_x"></param>
    /// <param name="offset_z"></param>
    private void GeneratePass(int offset_x, int offset_z) {
        foreach (Tuple<int, int> t in coordinateMapMin) {
            int x = t.Item1 + offset_x;
            int z = t.Item2 + offset_z;
            Chunk chunk = Chunk.GetChunk(x, z, CoordinateSpace.Chunk);
            Chunk temp;

            if (c_busy.TryGetValue(chunk.hashcode, out temp) ||
                c_done.TryGetValue(chunk.hashcode, out temp) ||
                c_generate.TryGetValue(chunk.hashcode, out temp)) {
                continue;
            } else if (c_remove.TryGetValue(chunk.hashcode, out temp)) {
                c_remove.Remove(chunk.hashcode);

                if (chunk.isGenerated) {
                    c_done.Add(chunk.hashcode, chunk);
                } else {
                    c_generate.Add(chunk.hashcode, chunk);
                }
            } else {
                c_generate.Add(chunk.hashcode, chunk);
            }

            if (!chunk.isInstantiated) {
                chunk.CreateInstances(chunkPrefab);
            }
        }
    }

    /// <summary>
    /// The remove (second) pass of the chunk manager's update.
    /// It determines the chunks that need to be removed.
    /// All chunks that need to be removed are correctly moved between the queues.
    /// </summary>
    /// <param name="offset_x"></param>
    /// <param name="offset_z"></param>
    private void RemovePass(int offset_x, int offset_z) {
        c_gBuffer.Clear();
        c_dBuffer.Clear();

        foreach (Tuple<int, int> t in coordinateMapMax) {
            int x = t.Item1 + offset_x;
            int z = t.Item2 + offset_z;

            if (Chunk.ChunkExists(Chunk.GetChunkHashCode(x, z, CoordinateSpace.Chunk))) {
                Chunk chunk = Chunk.GetChunk(x, z, CoordinateSpace.Chunk);
                Chunk temp;

                if (c_busy.TryGetValue(chunk.hashcode, out temp)) {
                    continue;
                } else if (c_generate.TryGetValue(chunk.hashcode, out temp)) {
                    c_generate.Remove(chunk.hashcode);
                    c_gBuffer.Add(chunk.hashcode, temp);
                } else if (c_done.TryGetValue(chunk.hashcode, out temp)) {
                    c_done.Remove(chunk.hashcode);
                    c_dBuffer.Add(chunk.hashcode, temp);
                } else if (c_remove.TryGetValue(chunk.hashcode, out temp)) {
                    c_remove.Remove(chunk.hashcode);

                    if (chunk.isGenerated) {
                        c_dBuffer.Add(chunk.hashcode, chunk);
                    } else {
                        c_gBuffer.Add(chunk.hashcode, chunk);
                    }
                } else {
                    throw new InvalidOperationException("Chunk exists but is not present in chunk manager's queues.");
                }
            }
        }

        keyBuffer.Clear();
        foreach (ulong key in c_done.Keys) {
            Chunk temp;
            c_done.TryGetValue(key, out temp);
            keyBuffer.Add(key);
            c_remove.Add(key, temp);
        }
        foreach (ulong key in keyBuffer) {
            c_done.Remove(key);
        }

        keyBuffer.Clear();        
        foreach (ulong key in c_generate.Keys) {
            Chunk temp;
            c_generate.TryGetValue(key, out temp);
            keyBuffer.Add(key);
            c_remove.Add(key, temp);
        }
        foreach (ulong key in keyBuffer) {
            c_generate.Remove(key);
        }

        Dictionary<ulong, Chunk> tempBuffer = c_gBuffer;
        c_gBuffer = c_generate;
        c_generate = tempBuffer;

        tempBuffer = c_dBuffer;
        c_dBuffer = c_done;
        c_done = tempBuffer;
    }

    /// <summary>
    /// The build (third) pass of the chunk manager.
    /// The Priority Queue for generation is rebuilt here.
    /// </summary>
    private void BuildPass(int offset_x, int offset_z) {
        this.chunkQueue.Clear();

        foreach (Chunk chunk in this.c_generate.Values) {
            PriorityQueueNode<Chunk> node = GetNode(chunk);
            this.chunkQueue.Enqueue(node, (int) Distance(new Tuple<int, int>(offset_x, offset_z), 
                new Tuple<int, int>(chunk.coordinates.GetX(CoordinateSpace.Chunk),
                chunk.coordinates.GetZ(CoordinateSpace.Chunk))));
        }
    }

    private PriorityQueueNode<Chunk> GetNode (Chunk chunk) {
        PriorityQueueNode<Chunk> node;
        if (!c_nodes.TryGetValue(chunk.hashcode, out node)) {
            node = new PriorityQueueNode<Chunk>(chunk);
        }
        return node;
    }

    private void UpdateChunkGeneration() {
        int origin_x = (int)origin.position.x >> 4;
        int origin_z = (int)origin.position.z >> 4;

        GeneratePass(origin_x, origin_z);
        RemovePass(origin_x, origin_z);
        BuildPass(origin_x, origin_z);
    }

    private void UpdateChunkRemoval() {
        int chunksRemoved = 0;

        keyBuffer.Clear();

        foreach (Chunk chunk in c_remove.Values) {
            if (c_remove.Keys.Count - chunksRemoved == 0 || chunksRemoved == chunkRemoveCount) {
                break;
            }
            keyBuffer.Add(chunk.hashcode);
            chunk.Destroy();
            chunksRemoved++;
        }

        foreach (ulong key in keyBuffer) {
            c_remove.Remove(key);
        }
    }

    private void UpdateChunkJobs() {
        if (chunkRemoveTimer >= chunkRemoveConstant) {
            chunkRemoveTimer -= 0.25;
            UpdateChunkRemoval();
        }

        chunkRemoveTimer += Time.deltaTime;
    }

    // Update is called once per frame
    void Update () {
        UpdateChunkJobs();
        UpdateTasks();
	}


    /// <summary>
    /// Compute a list of points in a circular area around an origin point (0, 0), sorted by the distance to origin.
    /// </summary>
    /// <param name="radius">The radius of the circle area.</param>
    /// <returns>The points residing inside a circle with radius being the one specified as input.</returns>
    public List<Tuple<int, int>> ComputeCoordinateMap(int radius) {
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
    /// Calculate the distance between two arbitary points.
    /// </summary>
    /// <param name="p1">Tuple containing the origin point's X and Y coordinates.</param>
    /// <param name="p2">Tuple containing the compared-against point's X and Y coordinates.</param>
    /// <returns>The distance between the two points.</returns>
    private static double DistanceSqrt(Tuple<int, int> p1, Tuple<int, int> p2) {
        return Math.Sqrt(Math.Pow(p2.Item1 - p1.Item1, 2) + Math.Pow(p2.Item2 - p1.Item2, 2));
    }

    /// <summary>
    /// Calculate the squared distance between two arbitary points. Faster to calculate than getting the actual distance as with DistanceSqrt().
    /// </summary>
    /// <param name="p1">Tuple containing the origin point's X and Y coordinates.</param>
    /// <param name="p2">Tuple containing the compared-against point's X and Y coordinates.</param>
    /// <returns>The squared distance between the two points.</returns>
    private static double Distance(Tuple<int, int> p1, Tuple<int, int> p2) {
        return Math.Pow(p2.Item1 - p1.Item1, 2) + Math.Pow(p2.Item2 - p1.Item2, 2);
    }
}