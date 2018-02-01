using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform followedObject;

    private void LateUpdate() {
        transform.position = followedObject.position;
        transform.rotation = followedObject.rotation;
    }
}
