using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectInfo : MonoBehaviour {
    public List<Mesh> meshes;
    void Awake() {
        List<MeshFilter> mfs = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
        foreach (MeshFilter mf in mfs) {
            if (mf.mesh.isReadable) {
                meshes.Add(mf.mesh);
            } else {
                print("mesh is not readable: " + mf.mesh.name);
            }
        }
    }

    public List<Vector3> getRandomTri() {
        int totalNumTris = 0;
        foreach (Mesh m in meshes) {
            totalNumTris += m.triangles.Length;
        }
        int randomIndex = Random.Range(0, totalNumTris);
        foreach (Mesh m in meshes) {
            if (m.triangles.Length > randomIndex) {
                return triCorners(m, randomIndex);
            }
            randomIndex -= m.triangles.Length;
        }
        return new List<Vector3>();//should never reach here
    }

    public List<Vector3> triCorners(Mesh mesh, int triIndex) {
        List<int> vertexIndices = new List<int> {
            mesh.triangles[triIndex],
            mesh.triangles[triIndex + 1],
            mesh.triangles[triIndex + 1]
        };
        List<Vector3> result = new List<Vector3> {
            mesh.vertices[vertexIndices[0]],
            mesh.vertices[vertexIndices[1]],
            mesh.vertices[vertexIndices[2]]
        };
        return result;
    }
}
