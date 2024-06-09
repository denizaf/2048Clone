using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; } // Singleton instance

    public GameObject tilePrefab;
    public Transform gridParent;
    public int gridSize = 4;
    public Text scoreText;
    
    private Tile[,] _grid;
    private int _score = 0;
    private float _tileSpacing = 5f; // Adjust based on your tile size and spacing
    
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
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
                _grid[x, y] = null;
            }
        }
    }
    
    private void SpawnTile()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (_grid[x, y] == null)
                {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (emptyPositions.Count > 0)
        {
            Vector2Int randomPos = emptyPositions[Random.Range(0, emptyPositions.Count)];
            GameObject tileObject = Instantiate(tilePrefab, gridParent);
            Tile newTile = tileObject.GetComponent<Tile>();
            newTile.Initialize(randomPos.x, randomPos.y, Random.value < 0.9f ? 2 : 4);
            _grid[randomPos.x, randomPos.y] = newTile;
            newTile.transform.localPosition = GetWorldPosition(randomPos.x, randomPos.y);
            newTile.transform.localScale = Vector3.zero;
            newTile.transform.DOScale(Vector3.one, 0.3f);
        }

       
    }
    
    public Vector3 GetWorldPosition(int x, int y)
    {
        RectTransform gridRect = gridParent.GetComponent<RectTransform>();
        float tileSize = (gridRect.rect.width - _tileSpacing * (gridSize - 1)) / gridSize;
        float startX = -(gridRect.rect.width / 2) + (tileSize / 2);
        float startY = -(gridRect.rect.height / 2) + (tileSize / 2);

        return new Vector3(startX + x * (tileSize + _tileSpacing), startY + y * (tileSize + _tileSpacing), 0);
    }

    public void MoveTiles(Vector2 direction)
    {
        bool hasMoved = false;
        
        

        if (direction == Vector2.down)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 1; y < gridSize; y++)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }
        else if (direction == Vector2.up)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = gridSize - 2; y >= 0; y--)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }
        else if (direction == Vector2.right)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = gridSize - 2; x >= 0; x--)
                {
                    hasMoved |= MoveTile(x, y, direction);
                }
            }
        }
        else if (direction == Vector2.left)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 1; x < gridSize; x++)
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
            if(tile != null)
                tile.SetMerged(false);
        }
    }

    private bool MoveTile(int x, int y, Vector2 direction)
    {
        Tile currentTile = _grid[x, y];

        if (currentTile == null)
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

            if (nextTile == null)
            {
                _grid[newX, newY] = currentTile;
                _grid[x, y] = null;
                currentTile.SetGridPosition(newX, newY);
                currentTile.transform.DOLocalMove(GetWorldPosition(newX, newY), 0.2f);
                hasMoved = true;
                x = newX;
                y = newY;
            }
            else if (nextTile.GetValue() == currentTile.GetValue() && !nextTile.HasMerged() && !currentTile.HasMerged())
            {
                int newValue = nextTile.GetValue() * 2;
                nextTile.SetValue(newValue);
                Destroy(currentTile.gameObject);
                _grid[x, y] = null;
                currentTile.transform.DOLocalMove(nextTile.transform.localPosition, 0.2f).OnComplete(() =>
                {
                    nextTile.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
                });
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
