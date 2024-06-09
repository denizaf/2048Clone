using UnityEngine.UI;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Text valueText;
    public Image tileBackground;

    private static readonly Color[] colors = {
        new Color(0.96f, 0.91f, 0.78f),  // 2
        new Color(0.96f, 0.88f, 0.68f),  // 4
        new Color(0.95f, 0.69f, 0.47f),  // 8
        new Color(0.95f, 0.58f, 0.39f),  // 16
        new Color(0.95f, 0.49f, 0.37f),  // 32
        new Color(0.96f, 0.37f, 0.24f),  // 64
        new Color(0.93f, 0.81f, 0.45f),  // 128
        new Color(0.93f, 0.80f, 0.38f),  // 256
        new Color(0.93f, 0.78f, 0.31f),  // 512
        new Color(0.93f, 0.76f, 0.24f),  // 1024
        new Color(0.93f, 0.74f, 0.18f),  // 2048

    };

    private int _x;
    private int _y;
    private int _value;
    private bool _isMerged;

    public void Initialize(int x, int y, int value)
    {
        _x = x;
        _y = y;
        SetValue(value);
        SetMerged(false);
    }

    public void SetMerged(bool merged)
    {
        _isMerged = merged;
    }
    
    public void SetGridPosition(int x, int y)
    {
        _x = x;
        _y = y;
        UpdatePosition();
    }

    public bool HasMerged()
    {
        return _isMerged;
    }

    public bool IsEmpty()
    {
        return _value == 0;
    }

    public void SetValue(int newValue)
    {
        _value = newValue;
        valueText.text = _value > 0 ? _value.ToString() : "";
        UpdateColor();
    }
    
    private void UpdateColor()
    {
        int index = Mathf.FloorToInt(Mathf.Log(_value, 2)) - 1;
        index = Mathf.Clamp(index, 0, colors.Length - 1);
        tileBackground.color = colors[index];
    }

    public int GetValue()
    {
        return _value;
    }
    
    private void UpdatePosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = GridManager.Instance.GetWorldPosition(_x, _y);
    }
}
