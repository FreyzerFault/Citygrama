using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{
    public List<Tile> tiles = new List<Tile>();

    public int width = 10;
    public int height = 10;

    private void Awake()
    {
        Reset();
    }

    private void Start()
    {
        Reset();
    }

    public void Clear()
	{
		if (GetComponentsInChildren<Tile>() != null)
		{
			foreach (Transform tileChild in transform)
			{
				if (tileChild.gameObject != null)
				{

					Destroy(tileChild.gameObject);
				}
			}
		}
        tiles.Clear();
	}

    public void Reset()
	{        
		tiles.ForEach(
            (tile) => {
				// Clear Tile Children
				tile.ClearTile();
				// Set road neighbours
				if (tile is RoadTile road)
					road.neighbours = GetNeighbours(tile.Index);
			}
            );
    }
    
    public void RevealTile(int index)
    {
        RevealTile(index % width, index / width);
    }

    public void RevealTile(int x, int y)
    {
        Tile tile = GetTile(x,y);
        if (tile != null)
        {
            tile.Reveal();
        }
    }
    public void RevealTile(Vector2Int coord)
    {
        RevealTile(coord.x, coord.y);
    }

    public List<Tile> GetNeighbours(int x, int y)
    {
        // ORDEN:
        // TOP - BOTTOM - LEFT - RIGHT
        
        return new List<Tile>
        {
            GetTopTile(x, y),
            GetBottomTile(x, y),
            GetLeftTile(x,y),
            GetRightTile(x,y)
        };
    }

    public List<Tile> GetNeighbours(int index)
    {
        return GetNeighbours(index % width, index / width);
    }
    
    public Tile GetTile(int x, int y)
    {
        // Si esta fuera devuelve null
        if (x >= width || x < 0 || y >= height || y < 0)
        {
            return null;
        }
        
        return tiles[y * width + x];
    }

    public Tile GetTile(int index)
    {
        return tiles[index];
    }
    
    public Tile GetTopTile(int x, int y)
    {
        return GetTile(x, y - 1);
    }
    
    public Tile GetBottomTile(int x, int y)
    {
        return GetTile(x, y + 1);
    }
    
    public Tile GetLeftTile(int x, int y)
    {
        return GetTile(x - 1, y);
    }
    
    public Tile GetRightTile(int x, int y)
    {
        return GetTile(x + 1, y);
    }
}
