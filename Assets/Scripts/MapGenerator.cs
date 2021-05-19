using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public string seed;
    public bool useRandomSeed;
    public int smoothingIterations;
    public int smoothingStrength;
    public int width, height;
    [Range(0,100)]
    public int randomFillPercent;
    int[,] map; // defines a grid. if a tile value is ZERO the tile is empty, if the tile value is ONE the tile is a wall

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            GenerateMap();
        }
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        // smooth map
        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if(useRandomSeed)
        {
            seed = Time.time.ToString();
        }
        System.Random prng = new System.Random(seed.GetHashCode()); // prng = Pseudo Random Number Generator

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x,y] = 1;
                }else
                {
                    map[x,y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }    
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // rules for smoothing the map
                int neighbourWallTiles = GetSurroundingWallCount(x,y);
                if(neighbourWallTiles > smoothingStrength)
                {
                    map[x,y] = 1;
                }else if (neighbourWallTiles < smoothingStrength)
                {
                    map[x,y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for(int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for(int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if(neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if(neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }else
                {
                    wallCount++; // encourage walls at the border of the map
                }
            }    
        }
        return wallCount;
    }

    void OnDrawGizmos() 
    {
        if(map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x,y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + .5f, 0, -height/2 + y + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }    
            }
        }
    }

}
