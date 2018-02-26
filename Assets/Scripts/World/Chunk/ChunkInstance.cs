using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World.Chunk
{
    public class ChunkInstance : MonoBehaviour
    {
        public MeshFilter meshFilter;

        // Use this for initialization
        void Start()
        {
            meshFilter = meshFilter.GetComponent<MeshFilter>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DestroyInstance() {
            Destroy(gameObject);
        }
    }
}