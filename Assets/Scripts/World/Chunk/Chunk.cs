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
        public static readonly int height = 8;
        private List<ChunkSegment> chunkSegments;

        public Chunk(Vector2Int xz, Space space = Space.World) : this(xz.x, xz.y, space) { }

        public Chunk(int x, int z, Space space = Space.World) {
            if (space == Space.World) {
                x >>= 4;
                z >>= 4;
            }

            chunkSegments = new List<ChunkSegment>(height);

            for (int y = 0; y < height; y++) {
                ChunkCoordinates coordinates = new ChunkCoordinates(x, y * 16, z, Space.Chunk);
                ChunkSegment segment = new ChunkSegment(this, coordinates);
                chunkSegments.Add(segment);
            }
        }

        public List<ChunkInstance> CreateGameInstances(GameObject chunkInstancePrefab) {
            List<ChunkInstance> gameInstances = new List<ChunkInstance>();
            ChunkInstance instance = GameObject.Instantiate(chunkInstancePrefab, new Transform());
        }

        public void GenerateTerrain(NoiseGenerator noiseGen) {
            float[] heightmap = GenerateHeightmap(noiseGen);

            foreach (ChunkSegment segment in chunkSegments) {
                Voxel[] voxels = new Voxel[4096];
                int heightOffset = segment.coordinates.GetY(Space.World);

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
        }

        private float[] GenerateHeightmap(NoiseGenerator noiseGen) {
            float[] heightmap = new float[17 * 17];

            for (int y = 0; y < 17; y++) {
                for (int x = 0; x < 17; x++) {
                    heightmap[x + y * 17] = noiseGen.Generate(x, y) * 16 * height + 1;
                }
            }

            return heightmap;
        }
    }
}