using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class ChunkInstance : MonoBehaviour
    {
        public MeshFilter meshFilter;
        private ChunkSegment chunkSegment;

        public void SetChunkSegment(ChunkSegment chunkSegment) {
            this.chunkSegment = chunkSegment;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}