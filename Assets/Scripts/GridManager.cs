using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;
    public int gridSize = 4;

    private Tile[,] _grid;

    private void Start()
    {
        _grid = new Tile[gridSize, gridSize];
        InitializeGrid();
        SpawnTile();
        SpawnTile();
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, gridParent);
                Tile tile = tileObject.GetComponent<Tile>();
                tile.Initialize(x, y);
                _grid[x, y] = tile;
            }
        }
    }
    
    private void SpawnTile()
    {
        List<Tile> emptyTiles = new List<Tile>();

        foreach (Tile tile in _grid)
        {
            if (tile.IsEmpty())
            {
                emptyTiles.Add(tile);
            }
        }

        if (emptyTiles.Count > 0)
        {
            Tile randomTile = emptyTiles[Random.Range(0, emptyTiles.Count)];
            randomTile.SetValue(Random.value < 0.9f ? 2 : 4); // 90% chance to spawn 2, 10% chance to spawn 4
        }
    }

    public void MoveTiles(Vector2 direction)
    {
        bool hasMoved = false;

        if (direction == Vector2.up)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = gridSize - 1; y >= 0; y--)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }
        else if (direction == Vector2.down)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }
        else if (direction == Vector2.left)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }
        else if (direction == Vector2.right)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = gridSize - 1; x >= 0; x--)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }

        if (hasMoved)
        {
            SpawnTile();
        }
    }

    private bool MoveTile(int x, int y, Vector2 direction)
    {
        Tile currentTile = _grid[x, y];

        //if tile is empty do nothing
        if (currentTile.IsEmpty())
        {
            return false;
        }

        int newX = x + (int)direction.x;
        int newY = y + (int)direction.y;

        if (IsWithinGrid(newX, newY))
        {
            Tile nextTile = _grid[newX, newY];

            if (nextTile.IsEmpty())
            {
                //just slide
                nextTile.SetValue(currentTile.GetValue());
                currentTile.SetValue(0);
                return true;
            }
            else if (nextTile.GetValue() == currentTile.GetValue())
            {
                //merge
                nextTile.SetValue(nextTile.GetValue() * 2);
                currentTile.SetValue(0);
                return true;
            }
        }

        return false;
    }

    private bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }
}
