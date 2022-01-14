/*
 * 
 * bounce balls simulation many agents
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ball
{
    public Vector2 pos;
    public float angle;
}


public class BouncingBalls : MonoBehaviour
{
    public ComputeShader cs;
    ComputeBuffer cb;
    [Range(1, 1000)] public int ballCount = 1;
    int ballStructSize;
    [Range(1, 20)] public float radius;
    [Range(1, 1000)] public float bounds;

    // Start is called before the first frame update
    void Start()
    {
        ballStructSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Ball));

        cb = new ComputeBuffer(ballCount, ballStructSize);
        Ball[] balls = new Ball[ballCount];
        for (int i = 0; i < ballCount; i++)
        {
            float x = Random.Range(0, bounds);
            float y = Random.Range(0, bounds);
            float a = Random.Range(0, 2 * Mathf.PI);
            Ball b = new Ball() { pos = new Vector2(x, y), angle = a };
            balls[i] = b;
        }
        cb.SetData(balls);
    }

    // Update is called once per frame
    void Update()
    {
        // calculate ball positions
        int k = cs.FindKernel("CSMain");
        cs.SetFloat("bounds", bounds);
        cs.SetFloat("time", Time.fixedTime);
        cs.SetBuffer(k, "balls", cb);
        cs.Dispatch(k, (ballCount > 8 ? ballCount : 8)/8, 1, 1);
    }

    private void OnDrawGizmos()
    {
        if (cb != null)
        {
            // draw balls
            Ball[] b = new Ball[ballCount];
            cb.GetData(b);
            for (int i = 0; i < ballCount; i++)
            {
                Gizmos.DrawWireSphere(new Vector3(b[i].pos.x, b[i].pos.y, 0), 1);
            }
        }
    }

    void OnDestroy()
    {
        cb.Release();
    }
}
