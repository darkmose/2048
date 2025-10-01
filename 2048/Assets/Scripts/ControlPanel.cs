using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 _pointerDownPos;
    private Vector2 _delta;
    public event Action MoveDownEvent;
    public event Action MoveUpEvent;
    public event Action MoveLeftEvent;
    public event Action MoveRightEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDownPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _delta = eventData.position - _pointerDownPos;
        _delta.Normalize();
        var dot = Vector2.Dot(_delta, Vector2.up);

        if (Mathf.Abs(dot) > 0.5f)
        {
            var sign = Mathf.Sign(dot);
            if (sign > 0)
            {
                MoveUpEvent?.Invoke();
            }
            else
            {
                MoveDownEvent?.Invoke();
            }
        }
        else
        {
            dot = Vector2.Dot(_delta, Vector2.right);
            var sign = Mathf.Sign(dot);
            if (sign > 0)
            {
                MoveRightEvent?.Invoke();
            }
            else
            {
                MoveLeftEvent?.Invoke();
            }

        }
    }
}
