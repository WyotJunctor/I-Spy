﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerPlanetSticker : MonoBehaviour {
    Transform planet_pivot, player_pivot;
    RigidbodyFirstPersonController player;
    bool pivoted;
    float lerp_speed = 10;

    float pivot_reset_checkpoint = -1, pivot_reset_interval = 1f;


    // Start is called before the first frame update
    void Start() {
        planet_pivot = transform.FindDeepParent("planet_pivot");
        player_pivot = transform.FindDeepParent("player_pivot");
        player = GetComponent<RigidbodyFirstPersonController>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!player.m_IsGrounded) {
            planet_pivot.parent = player_pivot.parent;
            pivoted = false;
        }
        if (pivoted) {
            player_pivot.position = planet_pivot.position;
            player_pivot.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(planet_pivot.forward, transform.up));
            //player_pivot.rotation = planet_pivot.rotation;
            //Game.SetGravity(-transform.up);
        }

        if (Time.time > pivot_reset_checkpoint) {
            pivot_reset_checkpoint = Time.time + pivot_reset_interval;
            planet_pivot.position = transform.position;
            player_pivot.position = transform.position;
            transform.position = player_pivot.position;
        }

    }

    private void OnCollisionEnter(Collision collision) {
        if (!player.m_IsGrounded) {
            return;
        }
        if (collision.collider.transform == planet_pivot.parent) {
            return;
        }
        pivoted = true;
        planet_pivot.parent = collision.collider.transform;
        planet_pivot.position = transform.position;
        player_pivot.position = transform.position;
        transform.position = player_pivot.position;
    }
}