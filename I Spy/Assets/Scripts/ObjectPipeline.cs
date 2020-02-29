#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;
using UnityEditor;

public class ObjectPipeline : MonoBehaviour {
    [Header("Asset Stuff")]
    public bool WINDOWS = true;
    public bool dump_planetoids = false;
    public GameObject prefab_root;
    public PlanetoidScriptableObjectList planetoid_list;
    public float target_size = 4f;

    [Header("Rigidbody Settings")]
    public float mass = 1000;
    public float drag = 100;
    public float angular_drag = 0;
    public PhysicMaterial player_collider_material;

    /// <summary>
    /// Retrieves selected folder on Project view.
    /// </summary>
    /// <returns></returns>
    public static string GetSelectedPathOrFallback(string provided_path) {
        string path = provided_path;

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    /// <summary>
    /// Recursively gather all files under the given path including all its subfolders.
    /// </summary>
    public static IEnumerable<string> GetFiles(string path, string ignore_path = "$$$$$") {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(path);
        while (queue.Count > 0) {
            path = queue.Dequeue();
            try {
                foreach (string subDir in Directory.GetDirectories(path)) {
                    if (!subDir.Contains(ignore_path))
                        queue.Enqueue(subDir);
                }
            }
            catch (System.Exception ex) {
                Debug.LogError(ex.Message);
            }
            string[] files = null;
            try {
                files = Directory.GetFiles(path);
            }
            catch (System.Exception ex) {
                Debug.LogError(ex.Message);
            }
            if (files != null) {
                for (int i = 0; i < files.Length; i++) {
                    yield return files[i];
                }
            }
        }
    }

    public void Process() {
        string slash = WINDOWS ? "\\" : "/";
        string source_path = $"Assets{slash}Objects{slash}Import";
        string target_path = $"Assets{slash}Objects{slash}Prefabs{slash}Planetoids";
        string primitive_target_path = $"Assets{slash}Objects{slash}Prefabs{slash}PrimitivePrefabs";
        string imported_path = $"Assets{slash}Objects{slash}Import";
        string ignore_path = $"Import{slash}Imported";

        // You can either filter files to get only neccessary files by its file extension using LINQ.
        // It excludes .meta files from all the gathers file list.
        var assetFiles = GetFiles(GetSelectedPathOrFallback(source_path), ignore_path).Where(s => s.Contains(".meta") == false);

        foreach (string f in assetFiles) {
            Debug.Log("Files: " + f);
            GameObject g = (GameObject)AssetDatabase.LoadAssetAtPath(f, typeof(GameObject));
            if (g == null) {
                continue;
            }
            string g_name = g.name;
            print(g_name);
            g = Instantiate(g);
            foreach (var comp in g.GetComponentsInChildren<Component>())
                if (!(comp is Transform) && !(comp is MeshFilter) && !(comp is MeshRenderer))
                    DestroyImmediate(comp);
            //print(g.GetComponentInChildren<MeshFilter>());
            GameObject main_obj = Instantiate(PrefabUtility.SaveAsPrefabAsset(g, $"{primitive_target_path}{slash}{g_name}.prefab"));
            main_obj.name = g_name;
            main_obj.layer = LayerMask.NameToLayer("PlanetoidCollision");

            Rigidbody rb = main_obj.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.mass = mass;
            rb.drag = drag;
            rb.angularDrag = angular_drag;

            Bounds extreme_bounds = new Bounds(Vector3.zero, Vector3.positiveInfinity);
            bool bounds_set = false;
            foreach (MeshFilter mf in main_obj.GetComponentsInChildren<MeshFilter>()) {
                //print(mf.name);
                var mesh = mf.sharedMesh;
                var mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.convex = true;
                mc.sharedMesh = mesh;
                mf.gameObject.layer = LayerMask.NameToLayer("PlanetoidCollision");

                GameObject player_collider = new GameObject();
                player_collider.transform.parent = mf.transform;
                player_collider.layer = LayerMask.NameToLayer("PlanetoidPlayerCollision");
                player_collider.name = $"{mf.gameObject.name}_PlayerCollider";
                player_collider.AddComponent<NonConvexCollider>();
                var player_mc = player_collider.GetComponent<MeshCollider>();
                player_mc.convex = false;
                player_mc.sharedMesh = mesh;
                player_mc.sharedMaterial = player_collider_material;
                player_collider.transform.localScale = Vector3.one;
                player_collider.transform.localRotation = Quaternion.identity;
                Bounds b = mf.GetComponent<MeshRenderer>().bounds;
                if (!bounds_set) {
                    //print("HAHAHA: " + b.size);
                    extreme_bounds = b;
                    bounds_set = true;
                } else {
                    extreme_bounds.SetMinMax(Vector3.Min(extreme_bounds.min, b.min), Vector3.Max(extreme_bounds.max, b.max));
                }
            }

            GameObject new_parent = Instantiate(prefab_root);
            main_obj.transform.parent = new_parent.transform;
            new_parent.name = main_obj.name;

            Vector3 size = extreme_bounds.size;
            //print("EXTENTS: " + size * 10000000f);
            float max_dim = new float[] { size[0], size[1], size[2] }.Max();
            float scale_factor = target_size / max_dim;
            main_obj.transform.localScale *= scale_factor;

            GameObject planetoid = PrefabUtility.SaveAsPrefabAsset(new_parent, $"{target_path}{slash}{new_parent.name}.prefab");
            string ext = Path.GetExtension(f);
            AssetDatabase.MoveAsset(f, $"{imported_path}{slash}{g_name}{ext}");

            if (!dump_planetoids)
                planetoid_list.planetoids.Add(planetoid);

            DestroyImmediate(new_parent);
            DestroyImmediate(g);
        }

        if (!dump_planetoids)
            return;
        planetoid_list.planetoids = new List<GameObject>();
        Populate();
    }


    public void Populate() {
        string slash = WINDOWS ? "\\" : "/";
        string target_path = $"Assets{slash}Objects{slash}Prefabs{slash}Planetoids";
        var planetoidFiles = GetFiles(GetSelectedPathOrFallback(target_path)).Where(s => s.Contains(".meta") == false);
        foreach (string f in planetoidFiles) {
            GameObject g = (GameObject)AssetDatabase.LoadAssetAtPath(f, typeof(GameObject));
            if (!planetoid_list.planetoids.Contains(g))
                planetoid_list.planetoids.Add(g);
        }
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(ObjectPipeline))]
public class ObjectPipelinerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var scripts = targets.OfType<ObjectPipeline>();
        if (GUILayout.Button("GARBULATE"))
            foreach (var script in scripts)
                script.Process();
        if (GUILayout.Button("POPULATE"))
            foreach (var script in scripts)
                script.Populate();
    }
}
#endif