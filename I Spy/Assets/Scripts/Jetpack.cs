using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {
    float thrust = 250f;

    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetButton("Jump")) {
            rb.AddForce(transform.up * thrust);
            rb.AddForce(transform.forward * thrust / 2);
        }
    }
}