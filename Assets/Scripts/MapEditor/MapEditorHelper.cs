/*
 * 
 * author: @jonhuo11
 * 
 * 2d map editor with custom tile support (WIP custom tile functionality)
 * Loads custom tiles for painting from persistent tile save folder
 * this is the ScriptableObject helper singleton asset that provides filesystem IO operations for the map editor
 * ensure only one of these objects exist in your project assets
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MapEditorHelper")]
public sealed class MapEditorHelper : ScriptableObject
{

    public static MapEditorHelper Instance { get; private set; }

    [SerializeField] string saveFolderName = "saves"; // storing map files

    // tilesets
    // TODO: implement loading of custom tilesets from a tilesets folder in persistentDataPath
    // for now just have static hardcoded list of tiles
    [SerializeField] string tilesetFolderName = "tilesets"; // storing custom tilesets for painting maps with
    [SerializeField] List<Tile> tilesToLoad = new List<Tile>();
    HashSet<Tile> tiles = new HashSet<Tile>();

    void LoadTileset()
    {
        tiles.Clear();
        foreach (Tile t in tilesToLoad)
        {
            if (!tiles.Add(t))
            {
                Debug.Log("identical tile already exists, skipping to next tile:" + t);
            }
        }
        Debug.Log("successfully loaded " + tiles.Count + " tiles into tile registry");
    }

    // ======================================= MAPEDITOR HELPER FUNCTIONS ==========================================

    public Tile[] GetTiles()
    {
        return tiles.ToArray();
    }

    public bool IsTileRecognized(Tile t)
    {
        return tiles.Contains(t);
    }

    public bool SaveMap()
    {
        return false;
    }

    public string GetSaveFolderLocation()
    {
        return Application.persistentDataPath + "/" + saveFolderName;
    }


    public FileInfo[] GetSaveFiles()
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(GetSaveFolderLocation());

            if (!di.Exists)
            {
                Directory.CreateDirectory(GetSaveFolderLocation());
                Debug.Log("did not find existing save folder, creating new one at " + GetSaveFolderLocation());
            }
            return di.GetFiles();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public string[] GetSaveFileNames()
    {
        FileInfo[] files = GetSaveFiles();
        List<string> output = new List<string>();
        foreach (FileInfo fi in files)
        {
        output.Add(fi.Name);
        }
        return output.ToArray();
    }

    // enforce singleton
    void OnEnable()
    {
        MapEditorHelper[] h = Resources.FindObjectsOfTypeAll<MapEditorHelper>();
        MapEditorHelper first = h.FirstOrDefault();
        Debug.Log("MapEditorHelpers found: " + ListToString(h));
        if (h.Length == 0)
        {
            Debug.LogError("found no MapEditorHelper asset, needs at least one in scene, using default");
        }
        else if (h.Length != 1)
        {
            Debug.LogError("found more than 1 MapEditorHelper asset in resources, there should only be one, will use first found:" + first);
        }
        Instance = first;

        // load the tileset
        LoadTileset();
    }

    public override string ToString()
    {
        return saveFolderName;
    }

    //  ================================== SOME STATIC HELPER FUNCTIONS =============================================

    public static string ListToString<T>(T[] ts)
    {
        return "[" + string.Join(", ", (ts.Select(p => p.ToString()).ToArray())) + "]";
    }

}
