using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerPlanetSticker : MonoBehaviour {
    Transform planet_pivot, player_pivot;
    RigidbodyFirstPersonController player;
    bool pivoted;
    float lerp_speed = 10;

    // Start is called before the first frame update
    void Start() {
        planet_pivot = transform.FindDeepParent("planet_pivot");
        player_pivot = transform.FindDeepParent("player_pivot");
        player = GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!player.m_IsGrounded) {
            if (planet_pivot.parent != player_pivot.parent) {
                print("player unpivoting from " + planet_pivot.parent.gameObject.name);
            }
            planet_pivot.parent = player_pivot.parent;
            pivoted = false;
        }
        if (pivoted) {
            player_pivot.position = planet_pivot.position;
            //player_pivot.position = Vector3.Lerp(transform.position, planet_pivot.position, Time.deltaTime * lerp_speed);
            player_pivot.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(planet_pivot.forward, Vector3.up));
        }
    }

    private void OnCollisionEnter(Collision collision) {
        print("player collided with " + collision.collider.gameObject.name);
        if (!player.m_IsGrounded) {
            //print("player colliding and not grounded");
            return;
        }
        if (collision.collider.transform == planet_pivot.parent) {
            print("player colliding again with previous pivot");
            return;
        }
        print("player pivoting on " + collision.collider.gameObject.name);
        pivoted = true;
        planet_pivot.parent = collision.collider.transform;
        planet_pivot.position = transform.position;
        player_pivot.position = transform.position;
        transform.position = player_pivot.position;
    }
}
