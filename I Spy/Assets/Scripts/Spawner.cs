using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public List<GameObject> objects;
    public int numObjects;
    public float maxSpawnDistance;
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < numObjects; i++) {
            GameObject obj = Instantiate(objects[Random.Range(0, objects.Count)], Random.onUnitSphere * Random.Range(0, maxSpawnDistance), Quaternion.Euler(Random.insideUnitSphere * 360));
            obj.transform.parent = transform;
            obj.transform.localScale = Vector3.one * Random.Range(700f, 1000f);
            obj.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
        }
    }
}
