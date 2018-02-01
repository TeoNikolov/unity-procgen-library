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
        private readonly int x;
        private readonly int z;

        public Chunk(Vector2Int xz, CoordinateSpace space = CoordinateSpace.World) : this(xz.x, xz.y, space) { }

        public Chunk(int x, int z, CoordinateSpace space = CoordinateSpace.World) {
            if (space == CoordinateSpace.World) {
                x >>= 4;
                z >>= 4;
            }

            this.x = x;
            this.z = z;

            chunkSegments = new List<ChunkSegment>(height);

            for (int y = 0; y < height; y++) {
                ChunkCoordinates coordinates = new ChunkCoordinates(x, y, z, CoordinateSpace.Chunk);
                ChunkSegment segment = new ChunkSegment(this, coordinates);
                chunkSegments.Add(segment);
            }
        }

        public void CreateInstances(GameObject prefab) {
            for (int i = 0; i < chunkSegments.Count; i++) {
                chunkSegments[i].CreateChunkInstance(prefab);
            }
        }

        public void GenerateTerrainAndMarch(NoiseGenerator noiseGen) {
            float[] heightmap = GenerateHeightmap(noiseGen);

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
        }

        private float[] GenerateHeightmap(NoiseGenerator noiseGen) {
            float[] heightmap = new float[17 * 17];

            for (int y = 0; y < 17; y++) {
                for (int x = 0; x < 17; x++) {
                    heightmap[x + y * 17] = noiseGen.Generate(x + this.x * 16, y + this.z * 16) * 16 * height + 1;
                }
            }

            return heightmap;
        }
    }
}