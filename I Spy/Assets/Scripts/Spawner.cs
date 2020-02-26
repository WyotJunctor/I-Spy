using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public List<GameObject> objects;
    public int numObjects;
    float minSpawnDistance = 50f;
    public float maxSpawnDistance;
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < numObjects; i++) {
            Vector3 pos = Random.onUnitSphere * Random.Range(minSpawnDistance, maxSpawnDistance);
            GameObject obj = Instantiate(objects[Random.Range(0, objects.Count)], pos, Quaternion.Euler(Random.insideUnitSphere * 360));
            obj.transform.parent = transform;
            obj.transform.localScale = Vector3.one * Random.Range(3f, 7f);
            obj.GetComponentInChildren<Rigidbody>().rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
        }
    }
}
