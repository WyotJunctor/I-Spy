using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public List<GameObject> objects;
    float minSpawnDistance = 30f;
    public float density;
    const float smallification = 0.0000001f;
    public float maxSpawnDistance;
    // Start is called before the first frame update
    void Start() {
        int numObjects = numObjectsFor(density, maxSpawnDistance);
        print(numObjects);
        for (int i = 0; i < numObjects; i++) {
            Vector3 pos;
            do {
                float x = Random.Range(-maxSpawnDistance, maxSpawnDistance);
                float y = Random.Range(-maxSpawnDistance, maxSpawnDistance);
                float z = Random.Range(-maxSpawnDistance, maxSpawnDistance);
                pos = new Vector3(x, y, z);
            } while (pos.magnitude <= minSpawnDistance);
            GameObject obj = Instantiate(randomObject(), pos, randomAngle());
            obj.transform.localScale = Vector3.one * Random.Range(3f, 7f);
            obj.transform.parent = transform;
            obj.GetComponentInChildren<Rigidbody>().rotation = randomAngle();
        }
    }

    int numObjectsFor(float dense, float dist) {
        return (int)(dense * smallification * Mathf.Pow(dist, 3));
    }

    GameObject randomObject() {
        return objects[Random.Range(0, objects.Count)];
    }

    Quaternion randomAngle() {
        return Quaternion.Euler(Random.insideUnitSphere * 360);
    }
}
