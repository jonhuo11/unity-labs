using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofFade : MonoBehaviour
{
    [SerializeField] string triggerTag;
    [SerializeField] float fadeTime;

    Material material;

    IEnumerator activeFade;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == triggerTag)
        {
            Debug.Log("triggering fade");

            if (activeFade != null)
            {
                StopCoroutine(activeFade);
            }
            activeFade = Fade(0);
            StartCoroutine(activeFade);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == triggerTag)
        {
            Debug.Log("stopping fade");

            if (activeFade != null)
            {
                StopCoroutine(activeFade);
            }
            activeFade = Fade(1);
            StartCoroutine(activeFade);
        }
    }

    IEnumerator Fade(float alphaTarget)
    {
        alphaTarget = Mathf.Clamp01(alphaTarget);
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float alpha = material.color.a;
            float lerped = Mathf.Lerp(alpha, alphaTarget, t / fadeTime);
            Color c = material.color;
            c.a = lerped;
            material.color = c;
            yield return null;
        }
    }
}
