using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    
    public GameObject tilePrefab;
    public int poolSize = 16;

    private Queue<GameObject> _tilePool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tile = Instantiate(tilePrefab,GridManager.Instance.gridParent);
            tile.SetActive(false);
            _tilePool.Enqueue(tile);
        }
    }

    public GameObject GetTile()
    {
        if (_tilePool.Count > 0)
        {
            GameObject tile = _tilePool.Dequeue();
            tile.SetActive(true);
            return tile;
        }
        else
        {
            GameObject tile = Instantiate(tilePrefab);
            return tile;
        }
    }

    public void ReturnTile(GameObject tile)
    {
        tile.SetActive(false);
        _tilePool.Enqueue(tile);
    }
}