using UnityEngine.UI;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Text valueText;

    private int _x;
    private int _y;
    private int _value;
    private bool _isMerged;

    public void Initialize(int x, int y)
    {
        _x = x;
        _y = y;
        SetValue(0);
        SetMerged(false);
    }

    public void SetMerged(bool merged)
    {
        _isMerged = merged;
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
    }

    public int GetValue()
    {
        return _value;
    }
}
