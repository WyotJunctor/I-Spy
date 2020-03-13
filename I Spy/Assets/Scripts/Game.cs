using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Game specific utils and enums
public enum PolygonMod {
    Checkpoint,
    Gravity,
    Teleport,
    Bond,
    BehaviorRoot
}

public static class Game {
    public static void SetGravity(Vector3 dir) {
        float gravMagnitude = Physics.gravity.magnitude;
        Physics.gravity = dir.normalized * gravMagnitude;
    }

    public static List<Vector3> TriCorners(MeshCollider meshCollider, int triIndex) {
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[triIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[triIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[triIndex * 3 + 2]];
        Transform hitTransform = meshCollider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        return new List<Vector3> { p0, p1, p2 };
    }
}
