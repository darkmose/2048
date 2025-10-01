using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameField : IDisposable
{
    public const float CELL_MOVE_DURATION = .2f;
    private readonly GameFieldCell[,] _cells;
    private bool _canMove = true;
    private Tweener _cellsMoveDelayTweener;
    public GameFieldCell[,] Cells => _cells;
    public event Action<GameFieldCell, GameFieldCell> CellMoveToCellEvent;
    public event Action<GameFieldCell, int> InitEvent;
    public event Action<int> GetScoreEvent;
    

    public GameField(Vector2Int fieldSize)
    {
        _cells = new GameFieldCell[fieldSize.x, fieldSize.y];

        for (int x = 0; x < fieldSize.x; x++)
        {
            for (int y = 0; y < fieldSize.y; y++)
            {
                var cell = new GameFieldCell(new Vector2Int(x, y))
                {
                    IsEmpty = true
                };

                _cells[x,y] = cell;
            }
        }
    }

    public void MoveLeft()
    {
            Debug.Log("MoveLeft");
        var moved = Move(true, false);
        var merged = CheckFieldForMerges(true, false);
        if (merged)
        {
            _canMove = true;
            moved |= Move(true, false);
        }
        if (moved)
        {
            DelayedMovePossibilityReset();
        }
        else
        {
            _canMove = true;
        }
    }

    public void MoveRight()
    {
            Debug.Log("MoveRight");
        var moved = Move(true, true);
        var merged = CheckFieldForMerges(true, true);
        if (merged)
        {
            _canMove = true;
            moved |= Move(true, true);
        }
        if (moved)
        {
            DelayedMovePossibilityReset();
        }
        else
        {
            _canMove = true;
        }
    }

    public void MoveUp()
    {
            Debug.Log("MoveUp");
        var moved = Move(false, true);
        var merged = CheckFieldForMerges(false, true);
        if (merged)
        {
            _canMove = true;
            moved |= Move(false, true);
        }
        if (moved)
        {
            DelayedMovePossibilityReset();
        }
        else
        {
            _canMove = true;
        }
    }

    public void MoveDown()
    {
            Debug.Log("MoveDown");
        var moved = Move(false, false);
        var merged = CheckFieldForMerges(false, false);
        if (merged)
        {
            _canMove = true;
            moved |= Move(false, false);
        }
        if (moved)
        {
            DelayedMovePossibilityReset();
        }
        else
        {
            _canMove = true;
        }
    }

    public void FillRandomCell()
    {
        var emptyCells = GetEmptyCells();
        if (emptyCells.Count == 0)
        {
            return;
        }

        var randomIndex = UnityEngine.Random.Range(0, emptyCells.Count);
        var cell = emptyCells[randomIndex];
        cell.Init(2);
        InitEvent?.Invoke(cell, 2);
    }

    private List<GameFieldCell> GetEmptyCells()
    {
        var list = new List<GameFieldCell>();
        foreach (var cell in _cells)
        {
            if (cell.IsEmpty)
            {
                list.Add(cell);
            }
        }
        return list;
    }

    private bool Move(bool isHorizontalMove, bool isPositiveDirection)
    {
        if (!_canMove)
        {
            return false;
        }
        _canMove = false;

        var sizeX = _cells.GetLength(0);
        var sizeY = _cells.GetLength(1);
        var motionDetected = false;
        var emptyExtremePoints = GetEmptyExtremePositions(isHorizontalMove, isPositiveDirection);

        if (isHorizontalMove)
        {
            if (isPositiveDirection)
            {
                for (int x = sizeX - 1; x >= 0; x--)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        var cell = _cells[x, y];
                        bool hasMove = TryMoveCell(isHorizontalMove, isPositiveDirection, sizeX, sizeY, emptyExtremePoints, x, y, cell);
                        motionDetected |= hasMove;
                        if (!hasMove)
                        {
                            continue;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        var cell = _cells[x, y];
                        bool hasMove = TryMoveCell(isHorizontalMove, isPositiveDirection, sizeX, sizeY, emptyExtremePoints, x, y, cell);
                        motionDetected |= hasMove;
                        if (!hasMove)
                        {
                            continue;
                        }
                    }
                }
            }
        }
        else
        {
            if (isPositiveDirection)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = sizeY - 1; y >= 0; y--)
                    {
                        var cell = _cells[x, y];
                        bool hasMove = TryMoveCell(isHorizontalMove, isPositiveDirection, sizeX, sizeY, emptyExtremePoints, x, y, cell);
                        motionDetected |= hasMove;
                        if (!hasMove)
                        {
                            continue;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        var cell = _cells[x, y];
                        bool hasMove = TryMoveCell(isHorizontalMove, isPositiveDirection, sizeX, sizeY, emptyExtremePoints, x, y, cell);
                        motionDetected |= hasMove;
                        if (!hasMove)
                        {
                            continue;
                        }
                    }
                }
            }
        }

        return motionDetected;
    }

    private void DelayedMovePossibilityReset()
    {
        _cellsMoveDelayTweener?.Kill();
        _cellsMoveDelayTweener = Timer.SetTimer(CELL_MOVE_DURATION, () =>
        {
            _canMove = true;
            FillRandomCell();
        });
    }

    private bool TryMoveCell(bool isHorizontalMove, bool isPositiveDirection, int sizeX, int sizeY, GameFieldCell[] emptyExtremePoints, int x, int y, GameFieldCell cell)
    {
        if (cell.IsEmpty)
        {
            return false;
        }

        if (isHorizontalMove)
        {
            if (isPositiveDirection)
            {
                var newCell = emptyExtremePoints[y];
                if (newCell == null || newCell.Pos.x < x)
                {
                    return false;
                }
                var prevCellPos = newCell.Pos + Vector2Int.left;

                if (!IsPositionInBounds(prevCellPos))
                {
                    return false;
                }

                var prevCell = _cells[prevCellPos.x, prevCellPos.y];
                emptyExtremePoints[y] = prevCell;

                MoveCellTo(cell, newCell);
            }
            else
            {
                var newCell = emptyExtremePoints[y];
                if (newCell == null || newCell.Pos.x > x)
                {
                    return false;
                }
                var prevCellPos = newCell.Pos + Vector2Int.right;

                if (!IsPositionInBounds(prevCellPos))
                {
                    return false;
                }

                var prevCell = _cells[prevCellPos.x, prevCellPos.y];
                emptyExtremePoints[y] = prevCell;

                MoveCellTo(cell, newCell);
            }
        }
        else
        {
            if (isPositiveDirection)
            {
                var newCell = emptyExtremePoints[x];
                if (newCell == null || newCell.Pos.y < y)
                {
                    return false;
                }
                var prevCellPos = newCell.Pos + Vector2Int.down;

                if (!IsPositionInBounds(prevCellPos))
                {
                    return false;
                }

                var prevCell = _cells[prevCellPos.x, prevCellPos.y];
                emptyExtremePoints[x] = prevCell;

                MoveCellTo(cell, newCell);
            }
            else
            {
                var newCell = emptyExtremePoints[x];
                if (newCell == null || newCell.Pos.y > y)
                {
                    return false;
                }
                var prevCellPos = newCell.Pos + Vector2Int.up;

                if (!IsPositionInBounds(prevCellPos))
                {
                    return false;
                }

                var prevCell = _cells[prevCellPos.x, prevCellPos.y];
                emptyExtremePoints[x] = prevCell;

                MoveCellTo(cell, newCell);
            }
        }

        return true;
    }

    private bool CheckFieldForMerges(bool isHorizontalMove, bool isPositiveDirection)
    {
        var sizeX = _cells.GetLength(0);
        var sizeY = _cells.GetLength(1);
        bool hasMerge = false;

        if (isHorizontalMove)
        {
            if (isPositiveDirection)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = sizeX - 1; x >= 0; x--)
                    {
                        var cell = _cells[x, y];
                        if (cell.IsEmpty)
                        {
                            continue;
                        }
                        if ((x - 1) >= 0)
                        {
                            var prevCell = _cells[x - 1, y];
                            if (!prevCell.IsEmpty && cell.Value == prevCell.Value)
                            {
                                MergeCellTo(prevCell, cell);
                                hasMerge = true;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        var cell = _cells[x, y];
                        if (cell.IsEmpty)
                        {
                            continue;
                        }
                        if ((x + 1) < sizeX)
                        {
                            var nextCell = _cells[x + 1, y];
                            if (!nextCell.IsEmpty && cell.Value == nextCell.Value)
                            {
                                MergeCellTo(nextCell, cell);
                                hasMerge = true;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (isPositiveDirection)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = sizeY - 1; y >= 0; y--)
                    {
                        var cell = _cells[x, y];
                        if (cell.IsEmpty)
                        {
                            continue;
                        }
                        if ((y - 1) >= 0)
                        {
                            var prevCell = _cells[x, y - 1];
                            if (!prevCell.IsEmpty && cell.Value == prevCell.Value)
                            {
                                MergeCellTo(prevCell, cell);
                                hasMerge = true;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        var cell = _cells[x, y];
                        if (cell.IsEmpty)
                        {
                            continue;
                        }
                        if ((y + 1) < sizeY)
                        {
                            var nextCell = _cells[x, y + 1];
                            if (!nextCell.IsEmpty && cell.Value == nextCell.Value)
                            {
                                MergeCellTo(nextCell, cell);
                                hasMerge = true;
                            }
                        }
                    }
                }
            }
        }

        return hasMerge;
    }

    private void MergeCellTo(GameFieldCell cell, GameFieldCell destination)
    {
        Debug.Log($"Merge Cell: cell:{cell.Pos} to destination: {destination.Pos}");
        var newValue = cell.Value + destination.Value;
        CellMoveToCellEvent?.Invoke(cell, destination);
        cell.Dispose();
        destination.Init(newValue);

        GetScoreEvent?.Invoke(newValue);
    }

    private bool IsPositionInBounds(Vector2 position)
    {
        return position.x >= 0 && position.x < _cells.GetLength(0) && position.y >= 0 && position.y < _cells.GetLength(1);
    }

    private void MoveCellTo(GameFieldCell cell, GameFieldCell destination)
    {
        Debug.Log($"Move Cell: cell:{cell.Pos} to destination: {destination.Pos}");
        var newValue = cell.Value + destination.Value;
        CellMoveToCellEvent?.Invoke(cell, destination);
        cell.Dispose();
        destination.Init(newValue);
    }

    /// <summary>
    /// Provides accessible points for movement depending on the direction of movement
    /// </summary>
    /// <returns></returns>
    private GameFieldCell[] GetEmptyExtremePositions(bool isHorizontalMove, bool isPositiveDirection)
    {
        var sizeX = _cells.GetLength(0);
        var sizeY = _cells.GetLength(1);

        if (isHorizontalMove)
        {
            var emptyCells = new GameFieldCell[sizeY];

            if (isPositiveDirection)
            {
                for (int x = sizeX - 1; x >= 0; x--)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        if (emptyCells[y] != null)
                        {
                            continue;
                        }

                        var cell = _cells[x, y];

                        if (cell.IsEmpty)
                        {
                            emptyCells[y] = cell;
                        }
                    }
                }

                return emptyCells;
            }
            else
            {
                for (int x = 0 ; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        if (emptyCells[y] != null)
                        {
                            continue;
                        }

                        var cell = _cells[x, y];

                        if (cell.IsEmpty)
                        {
                            emptyCells[y] = cell;
                        }
                    }
                }

                return emptyCells;
            }
        }
        else
        {
            var emptyCells = new GameFieldCell[sizeX];

            if (isPositiveDirection)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = sizeY - 1; y >= 0; y--)
                    {
                        if (emptyCells[x] != null)
                        {
                            continue;
                        }

                        var cell = _cells[x, y];

                        if (cell.IsEmpty)
                        {
                            emptyCells[x] = cell;
                        }
                    }
                }

                return emptyCells;
            }
            else
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        if (emptyCells[x] != null)
                        {
                            continue;
                        }

                        var cell = _cells[x, y];

                        if (cell.IsEmpty)
                        {
                            emptyCells[x] = cell;
                        }
                    }
                }

                return emptyCells;
            }
        }
    }

    public void Dispose()
    {
        var sizeX = _cells.GetLength(0);
        var sizeY = _cells.GetLength(1);

        for (int i = 0; i < sizeX; i++)
        {
            for (int k = 0; k < sizeY; k++)
            {
                var cell = _cells[i,k];
                cell.Dispose();
            }
        }
        _cellsMoveDelayTweener?.Kill();
        _canMove = true;
    }
}
