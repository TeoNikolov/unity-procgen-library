using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshData {
    private Vector3[] vertices;
    private List<int> triangles;
    private Vector3[] normals;

    public Vector3[] Vertices {
        get {
            if (vertices == null) {
                throw new NullReferenceException("Mesh Data Get ERROR: Vertices is NULL");
            }

            return vertices;
        }

        set { vertices = value; }
    }

    public List<int> Triangles {
        get {
            if (triangles == null) {
                throw new NullReferenceException("Mesh Data Get ERROR: Triangles is NULL");
            }

            return triangles;
        }

        set { triangles = value; }
    }

    public Vector3[] Normals {
        get {
            if (normals == null) {
                throw new NullReferenceException("Mesh Data Get ERROR: Normals is NULL");
            }

            return normals;
        }

        set { normals = value; }
    }
}
