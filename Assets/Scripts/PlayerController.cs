using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.World.Chunk;
using System;

public delegate void OnChunkBorderCrossedHandler();

public class PlayerController : MonoBehaviour {
    /// <summary>
    /// The radius around the player below which chunks will be loaded, measured in chunks.
    /// </summary>
    public int viewDistanceMin;

    /// <summary>
    /// The distance from the player beyond which chunks should be unloaded, measured in chunks.
    /// </summary>
    public int viewDistanceMax;

    public float movementSpeed;
    public float turboMovementSpeed;
    public float mouseSensitivity;

    private float xRotation;
    private float yRotation;

    private int xOldChunk;
    private int zOldChunk;

    private readonly static int borderCrossEventBorderCount = 2;

    private event OnChunkBorderCrossedHandler _onChunkBorderCrossed;

    public event OnChunkBorderCrossedHandler OnChunkBorderCrossed {
        add {
            _onChunkBorderCrossed += value;
        }
        remove {
            _onChunkBorderCrossed -= value;
        }
    }

    // Use this for initialization
    void Start() {
        xRotation = 0;
        yRotation = 0;
        xOldChunk = ChunkCoordinates2D.ConvertSpace((int)transform.position.x, CoordinateSpace.World);
        zOldChunk = ChunkCoordinates2D.ConvertSpace((int)transform.position.z, CoordinateSpace.World);
        _onChunkBorderCrossed.Invoke();
    }
    
    void Update() {
        int xChunk = ChunkCoordinates2D.ConvertSpace((int)transform.position.x, CoordinateSpace.World);
        int zChunk = ChunkCoordinates2D.ConvertSpace((int)transform.position.z, CoordinateSpace.World);

        if (Math.Abs(this.xOldChunk - xChunk) >= borderCrossEventBorderCount ||
            Math.Abs(this.zOldChunk - zChunk) >= borderCrossEventBorderCount) {
            xOldChunk = xChunk;
            zOldChunk = zChunk;
            _onChunkBorderCrossed.Invoke();
        }

        if (Input.GetKey(KeyCode.W)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                transform.position += transform.forward * turboMovementSpeed * Time.deltaTime;
            } else {
                transform.position += transform.forward * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.S)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                transform.position -= transform.forward * turboMovementSpeed * Time.deltaTime;
            } else {
                transform.position -= transform.forward * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.A)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                transform.position -= transform.right * turboMovementSpeed * Time.deltaTime;
            } else {
                transform.position -= transform.right * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.D)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                transform.position += transform.right * turboMovementSpeed * Time.deltaTime;
            } else {
                transform.position += transform.right * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.E)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                transform.position += transform.up * turboMovementSpeed * Time.deltaTime;
            } else {
                transform.position += transform.up * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.Q)) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                transform.position -= transform.up * turboMovementSpeed * Time.deltaTime;
            } else {
                transform.position -= transform.up * movementSpeed * Time.deltaTime;
            }
        }

        if (Input.GetButton("Fire1")) {
            xRotation += mouseSensitivity * Input.GetAxis("Mouse X");
            yRotation -= mouseSensitivity * Input.GetAxis("Mouse Y");
            yRotation = Mathf.Clamp(yRotation, -90, 90);
            transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
        }
    }
}
