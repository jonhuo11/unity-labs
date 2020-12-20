using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FighterCamera : MonoBehaviour
{
    [SerializeField] float lerpSpeed;

    void Update()
    {
        Vector3 from = Camera.main.transform.position;
        Vector3 to = new Vector3(transform.position.x, transform.position.y, from.z);
        Camera.main.transform.position = Vector3.Lerp(from, to, lerpSpeed * 0.01f);
    }
}
