using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World {
    public class Voxel {
        private byte value;

        public byte Value {
            get { return value; }
        }

        public Voxel() {
            this.value = 0;
        }

        public void SetCorner(int corner, bool state) {
            if (corner < 0 || corner > 7) {
                throw new ArgumentOutOfRangeException("Corner set ERROR: Corner parameter must be between 0 and 7 inclusive. Received: " + corner);
            }

            if (state) {
                value |= (byte)(1 << corner);
            } else {
                value &= (byte)~(1 << corner);
            }
        }

        public bool GetCornerState(int corner) {
            return (value & (byte)(1 << corner)) != 0;
        }

        public Vector3 GetEdgeOffset(int edge) {
            if (edge < 0 || edge > 11) {
                throw new ArgumentOutOfRangeException("Edge get ERROR: Edge parameter must be between 0 and 11 inclusive. Received: " + edge);
            }

            switch(edge) {
                default:
                    return new Vector3(0.5f, 0f, 0f);
                case 1:
                    return new Vector3(1f, 0f, 0.5f);
                case 2:
                    return new Vector3(0.5f, 0f, 1f);
                case 3:
                    return new Vector3(0f, 0f, 0.5f);
                case 4:
                    return new Vector3(0.5f, 1f, 0f);
                case 5:
                    return new Vector3(1f, 1f, 0.5f);
                case 6:
                    return new Vector3(0.5f, 1f, 1f);
                case 7:
                    return new Vector3(0f, 1f, 0.5f);
                case 8:
                    return new Vector3(0f, 0.5f, 0f);
                case 9:
                    return new Vector3(1f, 0.5f, 0f);
                case 10:
                    return new Vector3(1f, 0.5f, 1f);
                case 11:
                    return new Vector3(0f, 0.5f, 1f);
            }
        }
    }
}