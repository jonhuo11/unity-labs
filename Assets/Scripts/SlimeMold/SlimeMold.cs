/* WIP
 * 
 * Attempt 2 at making a slime mold simulation
 * 1. use a ComputeBuffer of agents, create agent kernel which walks agents from place to place and deposit
 *    footprint trails on map
 * 2. use a Texture (or another compute buffer) as trail and decay trail map kernel
 * 3. render map texture
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SlimeMold : MonoBehaviour
{
    public struct Agent
    {
        public Vector2 pos;
        public float angle; // in rads
    }

    public ComputeShader cs;
    RenderTexture map;
    ComputeBuffer agents;

    public int dim = 512;
    public int targetTps = 60;
    float tstep = 0.1f;
    float lastStep = 0f;

    [Header("Settings")]
    public float decayRate = 0.01f;
    public float step = 1f;

    Renderer r;

    void InitSim()
    {
        int k = cs.FindKernel("InitSim");
        cs.SetTexture(k, "map", map);
        cs.Dispatch(k, dim / 8, dim / 8, 1);

        // add some agents
        Vector2 p = new Vector2(dim / 2, dim / 2);
        float a = 0;
        Agent ag = new Agent() { pos = p, angle = a };
        Agent[] ags = { ag };

        agents = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Agent)));
        agents.SetData(ags);
    }

    void Step()
    {
        int k = 0;

        // set vars
        cs.SetInt("dim", dim);
        cs.SetFloat("time", Time.realtimeSinceStartup);
        cs.SetFloat("step", step);
        cs.SetFloat("decayRate", decayRate);

        // walk the agents
        
        k = cs.FindKernel("Walk");
        cs.SetBuffer(k, "agents", agents);
        cs.SetTexture(k, "map", map);
        cs.Dispatch(k, 1, 1, 1);

        // fade the map
        k = cs.FindKernel("Draw");
        cs.SetTexture(k, "map", map);
        cs.Dispatch(k, dim / 8, dim / 8, 1);

        // set texture
        r.material.SetTexture("_MainTex", map);
    }

    // Start is called before the first frame update
    void Start()
    {
        tstep = 1 / targetTps;

        // create render texture for trailmap
        map = new RenderTexture(dim, dim, 24);
        map.enableRandomWrite = true;

        // components
        r = GetComponent<Renderer>();
        r.enabled = true;

        InitSim();
    }

    // Update is called once per frame
    void Update()
    {
        lastStep += Time.deltaTime;
        if (lastStep >= tstep)
        {
            //Debug.Log("step");
            lastStep = 0f;
            Step();
        }
    }

    void OnDestroy()
    {
        agents.Release();
    }
}
