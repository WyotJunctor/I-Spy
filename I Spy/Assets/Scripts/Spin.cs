using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    Vector3 axis;
    float spinSpeed = 15;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        axis = Random.onUnitSphere;
        spinSpeed = Random.Range(spinSpeed / 2, spinSpeed);
        rb = GetComponent<Rigidbody>();
        float s = spinSpeed * Time.deltaTime;
        rb.angularVelocity = new Vector3(axis.x * s, axis.y * s, axis.z * s);
    }
}
