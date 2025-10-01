using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private Button _exitButton;
    public event Action ExitRequestEvent;

    private void Awake()
    {
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    public void SetScore(int score)
    {
        _score.text = score.ToString("Score: 0");
    }

    private void OnExitButtonClick()
    {
        ExitRequestEvent?.Invoke();
    }

    private void OnDestroy()
    {
        _exitButton.onClick.RemoveAllListeners();
    }
}
