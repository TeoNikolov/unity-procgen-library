using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public enum CoordinateSpace
    {
        Chunk,
        World
    }

    public class ChunkCoordinates3D
    {
        private readonly int x;
        private readonly int y;
        private readonly int z;

        public ChunkCoordinates3D(Vector3Int xyz, CoordinateSpace space = CoordinateSpace.World) : this(xyz.x, xyz.y, xyz.z, space) { }

        public ChunkCoordinates3D(int x, int y, int z, CoordinateSpace space = CoordinateSpace.World)
        {
            if (space == CoordinateSpace.World) {
                this.x = x >> 4;
                this.y = y >> 4;
                this.z = z >> 4;
            } else {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public Vector3Int GetXYZ(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return new Vector3Int(x << 4, y << 4, z << 4);
            } else {
                return new Vector3Int(x, y, z);
            }
        }

        public int GetX(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return x << 4;
            } else {
                return x;
            }
        }

        public int GetY(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return y << 4;
            } else {
                return y;
            }
        }

        public int GetZ(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return z << 4;
            } else {
                return z;
            }
        }
    }
}