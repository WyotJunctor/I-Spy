using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlanetSticker : MonoBehaviour
{
    Transform planet_pivot, player_pivot;
    bool pivoted;

    // Start is called before the first frame update
    void Start()
    {
        planet_pivot = transform.FindDeepParent("planet_pivot");
        player_pivot = transform.FindDeepParent("player_pivot");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player_pivot.position = planet_pivot.position;
        player_pivot.rotation = planet_pivot.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform == planet_pivot.parent)
            return;
        pivoted = true;
        planet_pivot.parent = collision.collider.transform;
        planet_pivot.position = transform.position;
        player_pivot.position = transform.position;
        transform.position = player_pivot.position;
    }
}
