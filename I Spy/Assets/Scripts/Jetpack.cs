using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {
    float thrust = 500f;

    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetButton("Jump")) {
            rb.AddForce(transform.up * thrust);
        }
    }
}