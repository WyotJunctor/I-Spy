using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController1 : MonoBehaviour
    {
        public float speed;
        public float jumpForce;

        public float groundCheckDistance;
        Vector3 groundContactNormal;

        public Camera cam;
        private Rigidbody rb;
        private CapsuleCollider capsule;
        public MouseLook mouseLook = new MouseLook();

        private bool jump, previouslyGrounded, jumping, grounded;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);
        }

        void Update()
        {
            RotateView();

            if (Input.GetButtonDown("Jump") && !jump)
            {
                jump = true;
            }
        }

        void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            //float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();
            if (input.magnitude > float.Epsilon)
            {
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, groundContactNormal).normalized;
                if (grounded)
                {
                    rb.velocity = desiredMove * speed;
                }
                else
                {
                    rb.velocity += desiredMove * speed * Time.deltaTime;
                }
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            previouslyGrounded = grounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, capsule.radius, -transform.up, out hitInfo,
                                   ((capsule.height / 2f) - capsule.radius) + groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                grounded = true;
                groundContactNormal = hitInfo.normal;
            }
            else
            {
                grounded = false;
                groundContactNormal = transform.up;
            }
            if (!previouslyGrounded && grounded && jumping)
            {
                jumping = false;
            }
        }

        private Vector2 GetInput()
        {
            Vector2 input = new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
            return input.normalized;
        }
    }
}
