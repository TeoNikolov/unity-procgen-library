using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class Chunk
    {
        /// <summary>
        /// The number of chunk segments a chunk has vertically.
        /// </summary>
        public static readonly int height = 4;
        private static Dictionary<ulong, Chunk> chunks = new Dictionary<ulong, Chunk>();
        public readonly ChunkCoordinates2D coordinates;
        public readonly ulong hashcode;

        [HideInInspector]
        public bool isGenerated = false;
        [HideInInspector]
        public bool isBeingGenerated = false;
        [HideInInspector]
        public bool isInstantiated = false;
        private List<ChunkSegment> chunkSegments;

        public Chunk(Vector2Int xz, CoordinateSpace space) : this(xz.x, xz.y, space) { }

        public Chunk(int x, int z, CoordinateSpace space) {
            if (space == CoordinateSpace.World) {
                x >>= 4;
                z >>= 4;
            }
            chunkSegments = new List<ChunkSegment>(height);
            coordinates = new ChunkCoordinates2D(x, z, CoordinateSpace.Chunk);
            hashcode = GetChunkHashCode();
            chunks.Add(hashcode, this);

            for (int y = 0; y < height; y++) {
                ChunkCoordinates3D coordinates = new ChunkCoordinates3D(x, y, z, CoordinateSpace.Chunk);
                ChunkSegment segment = new ChunkSegment(this, coordinates);
                chunkSegments.Add(segment);
            }

        }

        public void CreateInstances(GameObject prefab) {
            for (int i = 0; i < chunkSegments.Count; i++) {
                chunkSegments[i].CreateChunkInstance(prefab);
            }

            isInstantiated = true;
        }

        public void UpdateChunkMesh() {
            foreach (ChunkSegment segment in chunkSegments) {
                segment.ApplyMesh();
            }
        }

        public void GenerateTerrainAndMarch(NoiseGenerator noisegen) {
            float[] heightmap = GenerateHeightmap(noisegen);

            foreach (ChunkSegment segment in chunkSegments) {
                Voxel[] voxels = new Voxel[4096];
                int heightOffset = segment.coordinates.GetY(CoordinateSpace.World);

                for (int y = 0; y < 16; y++) {
                    for (int z = 0; z < 16; z++) {
                        for (int x = 0; x < 16; x++) {
                            Voxel voxel = new Voxel();
                            voxel.SetCorner(0, heightOffset + y <= heightmap[x + z * 17]);
                            voxel.SetCorner(1, heightOffset + y <= heightmap[x + 1 + z * 17]);
                            voxel.SetCorner(2, heightOffset + y <= heightmap[x + 1 + (z + 1) * 17]);
                            voxel.SetCorner(3, heightOffset + y <= heightmap[x + (z + 1) * 17]);
                            voxel.SetCorner(4, heightOffset + y + 1 <= heightmap[x + z * 17]);
                            voxel.SetCorner(5, heightOffset + y + 1 <= heightmap[x + 1 + z * 17]);
                            voxel.SetCorner(6, heightOffset + y + 1 <= heightmap[x + 1 + (z + 1) * 17]);
                            voxel.SetCorner(7, heightOffset + y + 1 <= heightmap[x + (z + 1) * 17]);
                            voxels[x + z * 16 + y * 256] = voxel;
                        }
                    }
                }

                segment.SetData(voxels);
                segment.GenerateMesh();
            }

            isGenerated = true;
            isBeingGenerated = false;
        }

        private float[] GenerateHeightmap(NoiseGenerator noisegen) {
            float[] heightmap = new float[17 * 17];

            for (int y = 0; y < 17; y++) {
                for (int x = 0; x < 17; x++) {
                    heightmap[x + y * 17] = noisegen.Generate(
                        x + coordinates.GetX(CoordinateSpace.World),
                        y + coordinates.GetZ(CoordinateSpace.World)) * 16 * height + 1;
                }
            }

            return heightmap;
        }

        /// <summary>
        /// If the chunk exists, returns a reference to the chunk at the provided cooridnates and null otherwise.
        /// </summary>
        /// <param name="x">The X coordinate for the chunk.</param>
        /// <param name="z">The Z coordinate for the chunk.</param>
        /// <param name="space">The space in which the X and Z coordinates are.</param>
        /// <returns>The existing chunk at the specific coordinates.</returns>
        public static Chunk GetChunk(int x, int z, CoordinateSpace space) {
            if (space == CoordinateSpace.Chunk) {
                x = ChunkCoordinates2D.ConvertSpace(x, CoordinateSpace.Chunk);
                z = ChunkCoordinates2D.ConvertSpace(z, CoordinateSpace.Chunk);
            }

            Chunk chunk;
            chunks.TryGetValue(GetChunkHashCode(x, z, CoordinateSpace.World), out chunk);

            if (chunk == null) {
                chunk = new Chunk(x, z, CoordinateSpace.World);
            }

            return chunk;
        }

        public static bool ChunkExists(ulong hashcode) {
            Chunk chunk;
            return chunks.TryGetValue(hashcode, out chunk);
        }

        /// <summary>
        /// Calculate the hash of the chunk based on its XZ coordinates.
        /// </summary>
        /// <returns>An unsigned long with the first 32 most significant bits being the Z world coordinate and the other 32 the X world coordinate.</returns>
        public ulong GetChunkHashCode() {
            return (ulong)coordinates.GetX(CoordinateSpace.World) + ((ulong)coordinates.GetZ(CoordinateSpace.World) << 32);
        }

        /// <summary>
        /// Calculates the proper hashcode for a chunk with x and z coordinates in a certain space.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <param name="space">Which space the coordinates are in</param>
        /// <returns>A long value to serve as the hashcode for the specific coordinate pair </returns>
        public static ulong GetChunkHashCode(int x, int z, CoordinateSpace space) {
            if (space == CoordinateSpace.Chunk) {
                x = ChunkCoordinates2D.ConvertSpace(x, CoordinateSpace.Chunk);
                z = ChunkCoordinates2D.ConvertSpace(z, CoordinateSpace.Chunk);
            }

            return (ulong)x + ((ulong)z << 32);
        }

        public void Destroy() {
            for (int i = 0; i < this.chunkSegments.Count; i++) {
                this.chunkSegments[i].Destroy();
            }

            this.chunkSegments.Clear();
            chunks.Remove(this.hashcode);
        }
    }
}