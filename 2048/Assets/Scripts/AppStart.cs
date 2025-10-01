using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UI
{
    Loading,
    Lobby,
    Game
}

public class AppStart : MonoBehaviour
{
    [SerializeField] private LobbyUI _lobbyUI;
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private GameFieldView _gameFieldView;
    [SerializeField] private GameConfiguration _gameConfiguration;
    [SerializeField] private ControlPanel _controlPanel;

    private GameController _gameManager;
    private GameField _gameField;


    private void Awake()
    {
        _lobbyUI.gameObject.SetActive(true);
        _lobbyUI.StartRequestEvent += StartRequestHandler;
        _lobbyUI.ContinueRequestEvent += ContinueRequestHandler;
        _gameUI.ExitRequestEvent += ExitRequestHandler;
        _gameField = new GameField(_gameFieldView.FieldSize);
        _gameManager = new GameController(_gameField, _gameFieldView, _gameConfiguration, _gameUI, _lobbyUI);
        UpdateUI(UI.Lobby);
    }

    private void SubscribeControlEvents()
    {
        _controlPanel.MoveDownEvent += _gameField.MoveDown;
        _controlPanel.MoveUpEvent += _gameField.MoveUp;
        _controlPanel.MoveLeftEvent += _gameField.MoveLeft;
        _controlPanel.MoveRightEvent += _gameField.MoveRight;
    }

    private void UnsubscribeControlEvents()
    {
        _controlPanel.MoveDownEvent -= _gameField.MoveDown;
        _controlPanel.MoveUpEvent -= _gameField.MoveUp;
        _controlPanel.MoveLeftEvent -= _gameField.MoveLeft;
        _controlPanel.MoveRightEvent -= _gameField.MoveRight;
    }

    private void ExitRequestHandler()
    {
        _gameManager.Dispose();
        UpdateUI(UI.Lobby);

        UnsubscribeControlEvents();
    }

    private void ContinueRequestHandler()
    {
        UpdateUI(UI.Game);
        _gameManager.StartGame();
        SubscribeControlEvents();
    }

    private void StartRequestHandler()
    {
        UpdateUI(UI.Game);
        _gameManager.StartGame();

        SubscribeControlEvents();
    }

    private void UpdateUI(UI ui)
    {
        switch (ui)
        {
            case UI.Loading:
                break;
            case UI.Lobby:
                _gameUI.gameObject.SetActive(false);
                _lobbyUI.gameObject.SetActive(true);
                break;
            case UI.Game:
                _lobbyUI.gameObject.SetActive(false);
                _gameUI.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        ExitRequestHandler();
    }
}
