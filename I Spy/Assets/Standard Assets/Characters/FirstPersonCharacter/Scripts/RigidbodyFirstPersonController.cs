using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour {
        [Serializable]
        public class MovementSettings {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            [HideInInspector] public float max_speed;
            [HideInInspector] public float maxMidairHorizSpeed;
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 1.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
            public bool Running {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody rb;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping; public bool m_IsGrounded;
        private float airControlFactor = 20f;

        LayerMask layer_mask;


        public Vector3 Velocity {
            get { return rb.velocity; }
        }

        public bool Grounded {
            get { return m_IsGrounded; }
        }

        public bool Jumping {
            get { return m_Jumping; }
        }

        public bool Running {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start() {
            rb = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);
            string[] goodLayers = { "PlanetoidPlyaerCollision" };
            layer_mask = ~LayerMask.GetMask(goodLayers);
            movementSettings.maxMidairHorizSpeed = movementSettings.ForwardSpeed * 2f;
        }


        private void Update() {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump) {
                m_Jump = true;
            }
        }

        void NormalizeHorizontalSpeed() {
            Vector3 velInPlane = Vector3.ProjectOnPlane(rb.velocity, transform.up);
            float verticalVel = Vector3.Dot(rb.velocity, transform.up);
            if (velInPlane.magnitude > movementSettings.maxMidairHorizSpeed) {
                velInPlane = velInPlane.normalized * movementSettings.maxMidairHorizSpeed;
            }
            rb.velocity = velInPlane + transform.up * verticalVel;
        }
        private void FixedUpdate() {
            GroundCheck();
            Vector2 input = GetInput();

            if (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove *= movementSettings.ForwardSpeed;

                if (m_IsGrounded) {
                    rb.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                } else if (advancedSettings.airControl) {
                    rb.AddForce(desiredMove * airControlFactor);
                    NormalizeHorizontalSpeed();
                }
            }

            if (m_IsGrounded) {
                rb.drag = 5f;

                if (m_Jump) {
                    rb.drag = 0f;
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    rb.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && rb.velocity.magnitude < 1f) {
                    rb.velocity = Vector3.zero;
                    rb.Sleep();
                }
            } else {
                rb.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping) {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }


        private float SlopeMultiplier() {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper() {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, layer_mask, QueryTriggerInteraction.Ignore)) {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f) {
                    rb.velocity = Vector3.ProjectOnPlane(rb.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput() {

            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            return input;
        }


        private void RotateView() {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck() {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), -transform.up, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, layer_mask, QueryTriggerInteraction.Ignore)) {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            } else {
                m_IsGrounded = false;
                m_GroundContactNormal = transform.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping) {
                m_Jumping = false;
            }
        }
    }
}
