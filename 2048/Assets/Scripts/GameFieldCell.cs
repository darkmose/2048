using System;
using UnityEngine;

public class GameFieldCell : IDisposable
{
    public bool IsEmpty = true;
    public int Value { get; private set; } = 0;
    public Vector2Int Pos;

    public GameFieldCell(Vector2Int pos)
    {
        Pos = pos;
    }

    public void Init(int value)
    {
        IsEmpty = false;
        Value = value;
    }

    public void Dispose()
    {
        IsEmpty = true;
        Value = 0;
    }
}
