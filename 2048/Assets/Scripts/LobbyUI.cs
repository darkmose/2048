using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _bestScore;
    public event Action StartRequestEvent;
    public event Action ContinueRequestEvent;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClick);
        _continueButton.onClick.AddListener(OnContinueButtonClick);
    }

    private void OnStartButtonClick()
    {
        StartRequestEvent?.Invoke();
    }

    private void OnContinueButtonClick()
    {
        ContinueRequestEvent?.Invoke();
    }

    public void SetBestScore(int bestScore)
    {
        _bestScore.text = bestScore.ToString("Best Score: 0");
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClick);
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
    }
}