using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public static class Utils {
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component {
        System.Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                try {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos) {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    public static T FindComponent<T>(this GameObject g, bool in_parent = true, bool in_children = true, int sibling_depth = 0) where T : Component {
        if (g.GetComponent<T>() != null) {
            return g.GetComponent<T>();
        } else if (in_children && g.GetComponentInChildren<T>() != null) {
            return g.GetComponentInChildren<T>();
        } else if (in_parent)
            if (g.GetComponentInParent<T>() != null)
                return g.GetComponentInParent<T>();

        GameObject current = g;
        while (sibling_depth > 0) {
            current = current.transform.parent.gameObject;
            if (!current)
                break;
            if (current.GetComponentInChildren<T>() != null) {
                return current.GetComponentInChildren<T>();
            }
            sibling_depth--;
        }

        return g.GetComponent<T>();
    }

    public static Vector3 NearestVertexTo(this MeshFilter mesh_filter, Vector3 point) {
        Mesh mesh = mesh_filter.sharedMesh;

        // convert point to local space
        point = mesh_filter.transform.InverseTransformPoint(point);

        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.vertices) {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr) {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        // convert nearest vertex back to world space
        return mesh_filter.transform.TransformPoint(nearestVertex);

    }

    public static void SetGravity(Vector3 dir) {
        float gravMagnitude = Physics.gravity.magnitude;
        Physics.gravity = dir.normalized * gravMagnitude;
    }
}

public static class TransformDeepChildExtension {
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName) {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0) {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static Transform FindDeepParent(this Transform aChild, string aName) {
        Transform node = aChild;
        while (node.parent) {
            Transform result = node.parent.Find(aName);
            if (result) return result;
            node = node.parent;
        }
        return null;
    }
}

public static class MeshExtensions {
    public static Mesh Copy(this Mesh mesh) {
        var copy = new Mesh();
        foreach (var property in typeof(Mesh).GetProperties()) {
            if (property.GetSetMethod() != null && property.GetGetMethod() != null) {
                property.SetValue(copy, property.GetValue(mesh, null), null);
            }
        }
        return copy;
    }
}