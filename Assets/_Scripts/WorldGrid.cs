using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
    public static WorldGrid Instance;
    public Tilemap NavGrid; //Point Top Hex
    public Tilemap FogGrid; //Point Top Hex
    public int Width, Length;
    public List<GameObject> TilePrefabs;
    public List<Tile> Tiles;
    public Tile FogTile;
    public float MapHeight = 5f , NoiseScale = 1f;
    Vector2 Offset;
    Texture2D HeightMap;
    public int HmapX, HmapY;
    public static Dictionary<Vector3, PathNode> PathMap;
    public Action OnWorldInit;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PathMap = new Dictionary<Vector3, PathNode>();
        GenerateHeight();
        InitWorld();
    }

    void InitWorld()
    {        

        for (int x = 0; x < Length; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                var gridCoords = new Vector3Int(x, z,0);
                var height = Vector3.up * MapHeight * SampleStepped(x, z);
                var worldPos = NavGrid.CellToWorld(gridCoords) + height / 2;
                //FogGrid.SetTile(newPos, FogTile);
                var index = GetTileByHeight(height.y);
                var tile = Instantiate(TilePrefabs[index], worldPos, Quaternion.identity, transform).GetComponent<PathNode>();
                tile.name = $"Hex {gridCoords}";
                tile.GridCoords = gridCoords;
                tile.WorldCoords = worldPos;
                if (index == TilePrefabs.Count -1) tile.Walkable = false;
                PathMap.Add(gridCoords, tile);
            }
        }

        foreach (var tile in PathMap)
        {
            tile.Value.CacheNeighbors();
        }
        FogGrid.RefreshAllTiles();
        OnWorldInit?.Invoke();
    }

    void GenerateHeight()
    {
        Offset = new Vector2(Random.Range(0, 9999), Random.Range(0, 9999));
        HeightMap = new Texture2D(HmapX, HmapY);

        for (int x = 0; x < HmapX; x++)
        {
            for (int y = 0; y < HmapY; y++)
            {
                HeightMap.SetPixel(x, y, SampleNoise(x, y));
            }
        }

        HeightMap.Apply();
    }

    Color SampleNoise(int x,int y)
    {
        float xCoord = (float)x / HmapX * NoiseScale + Offset.x;
        float yCoord = (float)y / HmapY * NoiseScale + Offset.y;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }

    public float SampleStepped(int x, int y)
    {
        int stepSizeX = HmapX / Width;
        int stepSizeY = HmapY / Length;

        return HeightMap.GetPixel((Mathf.FloorToInt(x * stepSizeX)), Mathf.FloorToInt(y * stepSizeY)).grayscale;
    }

    int GetTileByHeight(float height)
    {
        return Mathf.Clamp(Mathf.RoundToInt(height), 0, TilePrefabs.Count - 1);
    }

    public Vector3 GetTilePos(Vector3 Worldpos)
    {
        var gridPos = NavGrid.WorldToCell(Worldpos);
        var tilePos = NavGrid.CellToWorld(gridPos);
        var height = SampleStepped(gridPos.x, gridPos.y) * MapHeight;
        return new Vector3(tilePos.x, height / 2, tilePos.z);
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
