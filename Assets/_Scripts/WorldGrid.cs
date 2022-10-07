using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        GenerateHeight();
        InitWorld();
    }

    void InitWorld()
    {        

        for (int x = 0; x < Length; x++)
        {
            for (int z = 0; z < Width; z++)
            {
                var height = Vector3.up * MapHeight * SampleStepped(x, z);//Mathf.PerlinNoise(x / Width * Scale + Offset, z  / Length * Scale + Offset);
                var newPos = new Vector3Int(x, z);
                //FogGrid.SetTile(newPos, FogTile);
                //NavGrid.SetTile(newPos, Tiles[0]);
                //NavGrid.GetTile<Tile>(newPos).gameObject.transform.Translate(height);
                var index = GetTileByHeight(height.y);
                Instantiate(TilePrefabs[index], NavGrid.CellToWorld(newPos) + height, Quaternion.identity, transform);
                
            }
        }
        NavGrid.RefreshAllTiles();
        FogGrid.RefreshAllTiles();
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
