using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPolygon : MonoBehaviour {
    Camera cam;
    GameObject boi0;
    GameObject boi1;
    GameObject boi2;
    private void Start() {
        cam = Camera.main;
        boi0 = GameObject.Find("Boi0");
        boi1 = GameObject.Find("Boi1");
        boi2 = GameObject.Find("Boi2");
    }
    // Update is called once per frame
    void Update() {
        RaycastHit hit;
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            return;
        if (hit.triangleIndex < 0)
            return;
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Color green = Color.green;
        Debug.DrawLine(p0, p1, color: green);
        Debug.DrawLine(p1, p2, color: green);
        Debug.DrawLine(p2, p0, color: green);
        boi0.transform.position = p0;
        boi1.transform.position = p1;
        boi2.transform.position = p2;
    }
}
