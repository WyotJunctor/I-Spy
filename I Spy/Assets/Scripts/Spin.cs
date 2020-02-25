using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {
    Vector3 axis;
    public float spinSpeed;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        axis = Random.onUnitSphere;
        //spinSpeed = Random.Range(0, spinSpeed);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        //transform.Rotate(axis, spinSpeed * Time.deltaTime);
        rb.angularVelocity = new Vector3(spinSpeed/2f, spinSpeed, 0);
    }

    public float FindQuaternionTwist(Quaternion q, Vector3 axis)
    {
        //find orthogonal vector to axis
        //apply quaternion to it
        //project before and after onto plane which axis is normal to
        //find dot product of the two vectors
        //return acos(dot product)
        return 0f;
    }
    /*
    private static Matrix OrthoX = Matrix.CreateRotationX(MathHelper.ToRadians(90));
    private static Matrix OrthoY = Matrix.CreateRotationY(MathHelper.ToRadians(90));

    public static void FindOrthonormals(Vector3 normal, out Vector3 orthonormal1, out Vector3 orthonormal2)
    {
        Vector3 w = Vector3.Transform(normal, OrthoX);
        float dot = Vector3.Dot(normal, w);
        if (Math.Abs(dot) > 0.6)
        {
            w = Vector3.Transform(normal, OrthoY);
        }
        w.Normalize();

        orthonormal1 = Vector3.Cross(normal, w);
        orthonormal1.Normalize();
        orthonormal2 = Vector3.Cross(normal, orthonormal1);
        orthonormal2.Normalize();
    }

    public static float FindQuaternionTwist(Quaternion q, Vector3 axis)
    {
        axis.Normalize();

        // Get the plane the axis is a normal of
        Vector3 orthonormal1, orthonormal2;
        ExMath.FindOrthonormals(axis, out orthonormal1, out orthonormal2);

        Vector3 transformed = Vector3.Transform(orthonormal1, q);

        // Project transformed vector onto plane
        Vector3 flattened = transformed - (Vector3.Dot(transformed, axis) * axis);
        flattened.Normalize();

        // Get angle between original vector and projected transform to get angle around normal
        float a = (float)Math.Acos((double)Vector3.Dot(orthonormal1, flattened));

        return a;
    }
    */
}
