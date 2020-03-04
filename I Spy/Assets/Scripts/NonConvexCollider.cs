using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class NonConvexCollider : MonoBehaviour {
    Rigidbody master;
    // Start is called before the first frame update
    void Awake() {
        master = transform.parent.GetComponentInParent<Rigidbody>();
        var rb = GetComponent<Rigidbody>();
        if (master == null) {
            print("master is null for " + gameObject.name);
        }
        if (rb == null) {
            print("rb is null for " + gameObject.name);
        }
        rb.mass = master.mass;
        rb.drag = master.drag;
        rb.angularDrag = master.angularDrag;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }
}
