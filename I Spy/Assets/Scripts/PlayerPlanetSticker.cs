using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerPlanetSticker : MonoBehaviour {
    Transform planet_pivot, player_pivot;
    RigidbodyFirstPersonController player;
    bool pivoted;
    float lerp_speed = 10;

    float pivot_reset_checkpoint = -1, pivot_reset_interval = 1f;


    GameObject pivotIndicator;
    // Start is called before the first frame update
    void Start() {
        planet_pivot = transform.FindDeepParent("planet_pivot");
        player_pivot = transform.FindDeepParent("player_pivot");
        player = GetComponent<RigidbodyFirstPersonController>();
        pivotIndicator = transform.parent.Find("PivotIndicator").gameObject;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!player.m_IsGrounded) {
            if (planet_pivot.parent != player_pivot.parent) {
                print("player unpivoting from " + planet_pivot.parent.gameObject.name);
            }
            planet_pivot.parent = player_pivot.parent;
            pivoted = false;
            pivotIndicator.SetActive(false);
        }
        if (pivoted) {
            player_pivot.position = planet_pivot.position;
            //player_pivot.position = Vector3.Lerp(transform.position, planet_pivot.position, Time.deltaTime * lerp_speed);
            player_pivot.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(planet_pivot.forward, transform.up));
        }

        if (Time.time > pivot_reset_checkpoint) 
        {
            pivot_reset_checkpoint = Time.time + pivot_reset_interval;
            planet_pivot.position = transform.position;
            player_pivot.position = transform.position;
            transform.position = player_pivot.position;
        }

    }

    private void OnCollisionEnter(Collision collision) {
        print("player collided with " + collision.collider.gameObject.name);
        if (!player.m_IsGrounded) {
            print("player colliding and not grounded - probably the underside");
            return;
        }
        if (collision.collider.transform == planet_pivot.parent) {
            print("player colliding again with previous pivot");
            return;
        }
        print("player pivoting on " + collision.collider.gameObject.name);
        pivoted = true;
        pivotIndicator.SetActive(true);
        planet_pivot.parent = collision.collider.transform;
        planet_pivot.position = transform.position;
        player_pivot.position = transform.position;
        transform.position = player_pivot.position;
    }
}
