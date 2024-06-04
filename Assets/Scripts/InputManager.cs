using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GridManager _gridManager;

    private void Start()
    {
        _gridManager = GetComponent<GridManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _gridManager.MoveTiles(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _gridManager.MoveTiles(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _gridManager.MoveTiles(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _gridManager.MoveTiles(Vector2.left);
        }
    }
}
