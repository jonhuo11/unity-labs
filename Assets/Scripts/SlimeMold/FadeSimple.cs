/*
 * My first attempt at implementing a slime mold using compute shaders (inspired by Seb Lague)
 * Took most of my reference from Claire Blackshaw compute shader series
 * Basically followed tutorial and made a simple fade from white to black
 * To actually implement full slime mold we will need two kernels, one to walk agent and one to draw agent trails
 * cannot use texture pointer flipping technique
 *
 * note to self:
 * - material types are important and you should learn them
 * - unlit/texture can be used for simple color texture 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FadeSimple : MonoBehaviour
{

    public ComputeShader shader;
    public int textureSize = 512;

    public float targetFps = 10;
    float fstep;
    float lastStep = 0f;

    [Header("Fade settings")]
    [Range(0.01f, 0.9f)] public float decayRate = 0.01f; // decay per tick

    Renderer rend;
    RenderTexture[] renderTextures;
    const int textureCount = 2;
    int textureIndex = 0;
    ComputeBuffer agentBuffer;

    private void InitSim() // set whole board black
    {
        int kernel = shader.FindKernel("CSInitSim");
        shader.SetTexture(kernel, "Result", renderTextures[textureIndex]);
        shader.Dispatch(kernel, textureSize / 8, textureSize / 8, 1);
        rend.material.SetTexture("_MainTex", renderTextures[textureIndex]);
    }

    private void DoStep()
    {
        // we have a texture pointer
        // cycle texture which is previous, which is next
        // feed old texture as input and new texture as output
        // to avoid deep copy in memory, just overwrite unused
        int prevTextureIndex = textureIndex; // new prev
        textureIndex = (prevTextureIndex + 1) % 2; // new result

        int kernel = shader.FindKernel("CSMain");
        shader.SetTexture(kernel, "Prev", renderTextures[prevTextureIndex]);
        shader.SetTexture(kernel, "Result", renderTextures[textureIndex]);
        shader.SetFloat("decayRate", decayRate);

        shader.Dispatch(kernel, textureSize / 8, textureSize / 8, 1);
        rend.material.SetTexture("_MainTex", renderTextures[textureIndex]);
    }

    private void Start()
    {
        fstep = 1 / targetFps;

        renderTextures = new RenderTexture[textureCount];
        for (int i = 0; i < textureCount; i++)
        {
            renderTextures[i] = new RenderTexture(textureSize, textureSize, 24);
            renderTextures[i].enableRandomWrite = true;
            renderTextures[i].Create();
        }

        rend = GetComponent<Renderer>();
        rend.enabled = true;

        InitSim();
    }

    private void Update()
    {
        lastStep += Time.deltaTime;
        if (lastStep >= fstep)
        {
            
            lastStep = 0f;
            DoStep();
        }
    }

    // for each agent
    public struct Agent
    {
        public Vector2 pos;
        public Vector2 dir;
    }
}
