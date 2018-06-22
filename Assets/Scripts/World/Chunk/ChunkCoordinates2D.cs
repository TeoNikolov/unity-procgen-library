using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public enum CoordinateSpace {
        Chunk,
        World
    }

    public class ChunkCoordinates2D
    {
        private readonly int x;
        private readonly int z;

        public ChunkCoordinates2D(Vector2Int xz, CoordinateSpace space) : this(xz.x, xz.y, space) { }

        public ChunkCoordinates2D(int x, int z, CoordinateSpace space) {
            if (space == CoordinateSpace.World) {
                this.x = x >> 4;
                this.z = z >> 4;
            } else {
                this.x = x;
                this.z = z;
            }
        }

        public Vector2Int GetXZ(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return new Vector2Int(x << 4, z << 4);
            } else {
                return new Vector2Int(x, z);
            }
        }

        public int GetX(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return x << 4;
            } else {
                return x;
            }
        }

        public int GetZ(CoordinateSpace space = CoordinateSpace.Chunk) {
            if (space == CoordinateSpace.World) {
                return z << 4;
            } else {
                return z;
            }
        }

        /// <summary>
        /// Converts a value from one space to the other (world -> chunk OR chunk -> world).
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="space">The space in which the value is.</param>
        /// <returns>The converted value.</returns>
        public static int ConvertSpace(int value, CoordinateSpace space) {
            if (space == CoordinateSpace.Chunk) {
                return value << 4;
            } else {
                return value >> 4;
            }
        }
    }
}