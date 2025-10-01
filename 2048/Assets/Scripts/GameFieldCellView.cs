using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFieldCellView : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject _contentPanel;
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private Vector2Int _gridPosition;
    [SerializeField] private ColorProvider2048 _colorProvider;
    private Tweener _moveTweener;
    public Vector2Int GridPosition => _gridPosition;

    public void SetActive(bool active)
    {
        _contentPanel.SetActive(active);
    }

    public void SetValue(int value)
    {
        _value.text = value.ToString();
        var bgColor = _colorProvider.GetCellColor(value);
        var textColor = _colorProvider.GetTextColor(value);
        SetBackgroundColor(bgColor);
        SetTextColor(textColor);
    }

    public void MoveContentTo(Vector3 position, float duration, Action onComplete)
    {
        Debug.Log($"MoveContentTo: {position}");
        _moveTweener?.Kill();
        _moveTweener = _contentPanel.transform.DOMove(position, duration).OnComplete(() => 
        {
            Debug.Log($"MoveComplete: {position}");
            onComplete?.Invoke();
        });
    }

    private void SetBackgroundColor(Color color)
    {
        _background.color = color;
    }

    private void SetTextColor(Color color)
    {
        _value.color = color;
    }

    public void Dispose()
    {
        _contentPanel.transform.localPosition = Vector3.zero;
        SetActive(false);
    }
}