using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlatform : MonoBehaviour
{
    public MeshFilter filter;
    Transform platform;

    // Start is called before the first frame update
    void Awake()
    {
        platform = transform.FindDeepChild("Platform");
    }

    private void OnTriggerStay(Collider other)
    {
        //platform.position = filter.NearestVertexTo(other.transform.position);
        //platform.localRotation = Quaternion.Euler(-filter.NearestNormal(other.transform.position));
    }
}
