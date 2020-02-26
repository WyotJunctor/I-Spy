using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour {
    float thrust = 250f;
    float maxVel = 13f;

    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (Input.GetButton("Jump")) {
            rb.AddForce(transform.up * thrust);
        }
        if (rb.velocity.magnitude > maxVel) {
            rb.velocity = rb.velocity.normalized * maxVel;
        }
    }
}