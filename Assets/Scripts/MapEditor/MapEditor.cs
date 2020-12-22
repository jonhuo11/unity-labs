/*
 * 
 * author: @jonhuo11
 * 
 * map editor gameobject for creating 2d maps with custom tilesets (WIP custom tileset functionality)
 * uses MapEditorHelper.cs for ScriptableObject helper that provides functionality
 * 
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class MapEditor : MonoBehaviour
{

    public static MapEditor Instance { get; private set; }

    // tiling
    Tile activeTile;

    // layers
    List<Tilemap> layers = new List<Tilemap>();
    Tilemap activeLayer;
    int newLayerSortOrder = 0;

    Grid grid;

    /*
     * 
     * use unity native TilemapRenderer and Tilemap class for rendering the tileset    
     * 
     */

    // ================================================= LAYERS =======================================================
    void CreateLayer(string layerName)
    {
        if (transform.Find(layerName) == null)
        {
            // spawn a new layer
            GameObject newLayer = new GameObject();
            newLayer.transform.SetParent(transform);
            newLayer.name = layerName;

            newLayer.AddComponent<Tilemap>();
            newLayer.AddComponent<TilemapRenderer>();
            Tilemap l = newLayer.GetComponent<Tilemap>();

            l.GetComponent<TilemapRenderer>().sortingOrder = newLayerSortOrder;
            newLayerSortOrder++;

            layers.Add(l);
            activeLayer = l;

            Debug.Log("successfully added new layer named " + layerName);
        }
        else
        {
            Debug.Log("existing layer with name " + layerName + " was found");
        }
    }

    void CycleLayers()
    {
        activeLayer = layers[layers.IndexOf(activeLayer) + 1 >= layers.Count ? 0 : layers.IndexOf(activeLayer) + 1];
        Debug.Log("cycled layers, new active layer is now " + activeLayer.name);
    }

    // ================================================= PAINTING TILES ================================================
    Vector3Int GetGridPositionFromScreenPosition(Vector2 ss)
    {
        Vector3 ws = Camera.main.ScreenToWorldPoint(ss);
        Vector3Int tileCoords = new Vector3Int(Mathf.FloorToInt(ws.x), Mathf.FloorToInt(ws.y), 0);
        return grid.WorldToCell(tileCoords);
    }

    void PaintTile(Vector3Int coords, Tile t)
    {
        activeLayer.SetTile(coords, t);
        Debug.Log("set tile at " + coords + " to tile " + t + " on layer " + activeLayer.name);
    }

    void Update()
    {
        // when the user clicks, paint a tile there
        if (Input.GetMouseButtonUp(0))
        {
            Vector3Int mouseTilePos = GetGridPositionFromScreenPosition(Input.mousePosition);
            Debug.Log("mouse clicked at grid coords: " + mouseTilePos);

            if (activeTile != null)
            {
                PaintTile(mouseTilePos, activeTile);
            }
            else
            {
                Debug.LogError("no active tile found");
            }
        }

        // cycle between layers
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CycleLayers();
        }

        // create layer
        if (Input.GetKeyUp(KeyCode.C))
        {
            CreateLayer(Random.Range(0f, 999999999f).ToString());
        }
    }

    void Start()
    {
        grid = GetComponent<Grid>();

        // spawn a new layer to start working with
        CreateLayer("default");
        // select a initially equipped tile
        activeTile = MapEditorHelper.Instance.GetTiles()[0];
        if (activeTile == null)
            Debug.LogError("no tiles in registry available for equipping, check MapEditorHelper settings");

        // print out save files
        Debug.Log("Save files: " + MapEditorHelper.ListToString(MapEditorHelper.Instance.GetSaveFileNames()));
    }

    // enforce singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("found existing instance of MapEditor, deleting this instance's gameobject");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Debug.Log("MapEditor loaded");
        }
    }

}
