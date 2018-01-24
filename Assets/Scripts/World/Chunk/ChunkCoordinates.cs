using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public enum Space
    {
        Chunk,
        World
    }

    public class ChunkCoordinates
    {
        private readonly int x;
        private readonly int y;
        private readonly int z;

        public ChunkCoordinates(Vector3Int xyz, Space space = Space.World) : this(xyz.x, xyz.y, xyz.z, space) { }

        public ChunkCoordinates(int x, int y, int z, Space space = Space.World)
        {
            if (space == Space.World) {
                this.x = x >> 4;
                this.y = y >> 4;
                this.z = z >> 4;
            } else {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public Vector3Int GetXYZ(Space space = Space.Chunk) {
            if (space == Space.World) {
                return new Vector3Int(x << 4, y << 4, z << 4);
            } else {
                return new Vector3Int(x, y, z);
            }
        }

        public int GetX(Space space = Space.Chunk) {
            if (space == Space.World) {
                return x << 4;
            } else {
                return x;
            }
        }

        public int GetY(Space space = Space.Chunk) {
            if (space == Space.World) {
                return y << 4;
            } else {
                return y;
            }
        }

        public int GetZ(Space space = Space.Chunk) {
            if (space == Space.World) {
                return z << 4;
            } else {
                return z;
            }
        }
    }
}