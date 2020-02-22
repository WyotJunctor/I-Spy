using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    Vector3 axis;
    public float spinSpeed;
    // Start is called before the first frame update
    void Start() {
        axis = Random.onUnitSphere;
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(axis, spinSpeed * Time.deltaTime);
    }
}
