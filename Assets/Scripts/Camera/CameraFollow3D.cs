using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow3D : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 rotation;
    [SerializeField] Transform target;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
