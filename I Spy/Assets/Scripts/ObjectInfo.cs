using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour {
    public List<Mesh> meshes;
    public Dictionary<int, PolygonMod> specialPolygons;
    void Awake() {
        List<MeshFilter> mfs = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
        foreach (MeshFilter mf in mfs) {
            if (mf.mesh.isReadable) {
                meshes.Add(mf.mesh);
            } else {
                print("mesh is not readable: " + mf.mesh.name + " in " + gameObject.name);
            }
        }
    }

    public int totalNumTris() {
        int result = 0;
        foreach (Mesh m in meshes) {
            result += m.triangles.Length;
        }
        return result;
    }
}
