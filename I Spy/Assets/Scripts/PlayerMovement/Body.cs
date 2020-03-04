using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    float jump_multiplier = 3f;
    float fall_multiplier = 3.5f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        OnFall();
    }

    private void OnFall()
    {
        if (!rb.useGravity)
            return;
        if (rb.velocity.y < 0f)
        {
            rb.velocity += transform.up * Physics.gravity.y * (fall_multiplier - 1f) * Time.deltaTime;
        }
        if (rb.velocity.y > 0f)
        {
            rb.velocity += transform.up * Physics.gravity.y * (jump_multiplier - 1f) * Time.deltaTime;
        }
    }
}