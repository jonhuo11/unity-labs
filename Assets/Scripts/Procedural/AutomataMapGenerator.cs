/*
 * 
 * author: @jonhuo11
 * 
 * based on tutorial from Sebastian Lague --> https://www.youtube.com/watch?v=v7yyZZjF1z4
 * 
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class AutomataMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public Vector2Int dimensions;
    [Range(0, 100)] public int fillPercent;
    public string seed;
    public Color wallColor = Color.black; 
    public Color floorColor = Color.white;

    [Header("Smoothing Settings")]
    public int smoothingIterations = 1; // times to run the smoothing algorithm
    [Range(0,8)] public int smoothToWallThreshold = 4; // number of tiles surrounding a tile needed for a tile to be smoothed
    public bool useParallelSmoother = true; // removes diagonal bias if true

    bool[,] map;

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            map = GenerateMap();
        }
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    Gizmos.color = map[x, y] ? wallColor : floorColor;
                    Gizmos.DrawCube(transform.position + new Vector3(x, y, 0), Vector2.one);
                }
            }
        }
    }

    // generates new map
    bool[,] GenerateMap()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        bool[,] m = GenerateRandomlyFilledMap();
        for (int i = 0; i < smoothingIterations; i++)
        {
            if (useParallelSmoother)
                m = GetSmoothedMap(m);
            else
                m = GetSmoothedMapNonParallel(m);
        }
        watch.Stop();
        Debug.Log("map generated in " + watch.ElapsedMilliseconds + "ms");
        return m;
    }

    // fills an array grid randomly with 1 or 0, trying to stay roughly around % filled
    bool[,] GenerateRandomlyFilledMap()
    {
        bool[,] m = new bool[dimensions.x, dimensions.y];

        string s = seed;
        if (seed.Length <= 0)
            s = Time.time.ToString();
        System.Random rng = new System.Random(s.GetHashCode());

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                if (x >= dimensions.x - 1 || x <= 0 || y >= dimensions.y - 1 || y <= 0) // make edges walls
                {
                    m[x, y] = true;
                }
                else
                {
                    m[x, y] = rng.Next(0, 100) < fillPercent;
                }
            }
        }

        return m;
    }

    // smoothes a map, this is where cellular automata occurs
    // essentially counts number of neighbours of a certain type
    bool[,] GetSmoothedMap(bool[,] m)
    {
        bool[,] next = new bool[m.GetLength(0), m.GetLength(1)];

        for (int x = 0; x < m.GetLength(0); x++)
        {
            for (int y = 0; y < m.GetLength(1); y++)
            {
                int wc = GetNearbyWallCount(new Vector2Int(x, y), m);

                if (wc > smoothToWallThreshold)
                {
                    next[x, y] = true;
                }
                else if (wc < smoothToWallThreshold)
                {
                    next[x, y] = false;
                }
                else
                {
                    next[x, y] = m[x, y];
                }
            }
        }

        return next;
    }

    // smooths a map, but does not write smoothing changes to a new array, mutates current array instead
    // results in diagonal bias
    bool[,] GetSmoothedMapNonParallel(bool[,] m)
    {
        for (int x = 0; x < m.GetLength(0); x++)
        {
            for (int y = 0; y < m.GetLength(1); y++)
            {
                int wc = GetNearbyWallCount(new Vector2Int(x, y), m);

                if (wc > smoothToWallThreshold)
                {
                    m[x, y] = true;
                }
                else if (wc < smoothToWallThreshold)
                {
                    m[x, y] = false;
                }
                else
                {
                    m[x, y] = m[x, y];
                }
            }
        }
        return m;
    }

    // true is wall, false is empty, returns 8 at max since a tile can at most be surrounded by 8 other tiles
    int GetNearbyWallCount(Vector2Int pos, bool[,] m)
    {
        int count = 0;
        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                if (!(x == pos.x && y == pos.y))
                {
                    if (x < m.GetLength(0) && x > -1 && y < m.GetLength(1) && y > -1)
                    {
                        if (m[x, y])
                        {
                            count++;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

}
