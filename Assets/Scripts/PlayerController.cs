using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    public float turboMovementSpeed;
    public float mouseSensitivity;
    private float xRotation;
    private float yRotation;

    // Use this for initialization
    void Start () {
        xRotation = 0;
        yRotation = 0;
    }
	
	// Update is called once per frame
	void Update () {
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
                transform.position -= transform.right* movementSpeed * Time.deltaTime;
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

        //if (Input.GetButton("Fire1")) {
            xRotation += mouseSensitivity * Input.GetAxis("Mouse X");
            yRotation -= mouseSensitivity * Input.GetAxis("Mouse Y");
            yRotation = Mathf.Clamp(yRotation, -90, 90);
            transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
        //}
    }
}
