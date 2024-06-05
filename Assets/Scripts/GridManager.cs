using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform gridParent;
    public int gridSize = 4;
    public Text scoreText;
    
    private Tile[,] _grid;
    private int _score = 0;
    
    private void Start()
    {
        _grid = new Tile[gridSize, gridSize];
        InitializeGrid();
        SpawnTile();
        SpawnTile();
        UpdateScore(0);
    }

    private void UpdateScore(int points)
    {
        _score += points;
        scoreText.text = "Score: " + _score;
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
            ResetMergeState();
        }
    }

    private void ResetMergeState()
    {
        foreach (Tile tile in _grid)
        {
            tile.SetMerged(false);
        }
    }

    private bool MoveTile(int x, int y, Vector2 direction)
    {
        Tile currentTile = _grid[x, y];

        if (currentTile.IsEmpty())
        {
            return false;
        }

        bool hasMoved = false;
        
        int newX = x;
        int newY = y;

        while (true)
        {
            newX += (int)direction.x;
            newY += (int)direction.y;
            
            if (!IsWithinGrid(newX, newY))
            {
                break;
            }
            
            Tile nextTile = _grid[newX, newY];

            if (nextTile.IsEmpty())
            {
                nextTile.SetValue(currentTile.GetValue());
                currentTile.SetValue(0);
                currentTile = nextTile;
                hasMoved = true;
            }
            else if (nextTile.GetValue() == currentTile.GetValue() && !nextTile.HasMerged() && !currentTile.HasMerged())
            {
                int newValue = nextTile.GetValue() * 2;
                nextTile.SetValue(newValue);
                currentTile.SetValue(0);
                UpdateScore(newValue);
                nextTile.SetMerged(true);
                hasMoved = true;
                break;
            }
            else
            {
                break;
            }
        }
        
        return hasMoved;
    }

    private bool IsWithinGrid(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }
}
