using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameFieldView : MonoBehaviour, IDisposable
{
    [SerializeField] private Vector2Int _fieldSize;
    [SerializeField] private List<GameFieldCellView> _cells;
    private GameField _gameField;
    public Vector2Int FieldSize => _fieldSize;

    public void LinkModel(GameField gameField)
    {
        _gameField = gameField;
        gameField.CellMoveToCellEvent += CellMoveToCellHandler;
        gameField.InitEvent += CellInitHandler;
    }

    private void CellInitHandler(GameFieldCell cell, int newValue)
    {
        var pos = cell.Pos;
        var cellView = GetCellByPos(pos);
        cellView.SetActive(true);
        cellView.SetValue(newValue);
    }

    private void CellMoveToCellHandler(GameFieldCell targetCell, GameFieldCell destination)
    {
        var targetCellView = GetCellByPos(targetCell.Pos);
        var destinationCellView = GetCellByPos(destination.Pos);
        targetCellView.MoveContentTo(destinationCellView.transform.position, GameField.CELL_MOVE_DURATION, () => 
        {
            targetCellView.Dispose();
            destinationCellView.SetActive(true);
            destinationCellView.SetValue(destination.Value);
        });
    }

    private GameFieldCellView GetCellByPos(Vector2Int pos)
    {
        var cellView = _cells.Find(pred => pred.GridPosition == pos);
        if (cellView != null)
        {
            return cellView;
        }
        else
        {
            throw new Exception($"Cell view with position {pos} doesn't exist!");
        }
    }

    public void Dispose()
    {
        foreach (var cell in _cells)
        {
            cell.Dispose();
        }

        _gameField.CellMoveToCellEvent -= CellMoveToCellHandler;
        _gameField.InitEvent -= CellInitHandler;
    }
}
